namespace GameDemo;

using System;
using System.Runtime.CompilerServices;
using Chickensoft.AutoInject;
using Chickensoft.GodotNodeInterfaces;
using Chickensoft.LogicBlocks;
using Chickensoft.PowerUps;
using Godot;
using SuperNodes.Types;

public interface IPlayer :
  ICharacterBody3D, IKillable, ICoinCollector, IPushEnabled {
  public bool IsMovingHorizontally();

  /// <summary>
  ///   Uses the engine to determine the input vector, relative to the global
  ///   camera direction.
  /// </summary>
  /// <param name="cameraBasis">Camera's global transform basis.</param>
  public Vector3 GetGlobalInputVector(Basis cameraBasis);

  /// <summary>
  ///   Gets the player's next rotation basis, based on the normalized desired
  ///   global direction, the delta time, and the rotation speed.
  /// </summary>
  /// <param name="direction">Normalized global direction.</param>
  /// <param name="delta">Delta time.</param>
  /// <param name="rotationSpeed">Rotation speed (quaternions?/sec).</param>
  Basis GetNextRotationBasis(
    Vector3 direction,
    double delta,
    float rotationSpeed
  );
}

[SuperNode(typeof(Provider), typeof(Dependent), typeof(AutoNode))]
public partial class Player : CharacterBody3D, IPlayer, IProvide<IPlayerLogic> {
  public override partial void _Notification(int what);

  #region Provisions

  IPlayerLogic IProvide<IPlayerLogic>.Value() => PlayerLogic;

  #endregion Provisions

  #region Dependencies

  [Chickensoft.AutoInject.Dependency]
  public IGameRepo GameRepo => DependOn<IGameRepo>();

  [Chickensoft.AutoInject.Dependency]
  public IAppRepo AppRepo => DependOn<IAppRepo>();

  #endregion Dependencies

  #region Exports

  /// <summary>Rotation speed (quaternions?/sec).</summary>
  [Export(PropertyHint.Range, "0, 100, 0.1")]
  public float RotationSpeed { get; set; } = 12.0f;

  /// <summary>Stopping velocity (meters/sec).</summary>
  [Export(PropertyHint.Range, "0, 100, 0.1")]
  public float StoppingSpeed { get; set; } = 1.0f;

  /// <summary>Player gravity (meters/sec).</summary>
  [Export(PropertyHint.Range, "-100, 0, 0.1")]
  public float Gravity { get; set; } = -30.0f;

  /// <summary>Player speed (meters/sec).</summary>
  [Export(PropertyHint.Range, "0, 100, 0.1")]
  public float MoveSpeed { get; set; } = 8f;

  /// <summary>Player speed (meters^2/sec).</summary>
  [Export(PropertyHint.Range, "0, 100, 0.1")]
  public float Acceleration { get; set; } = 4f;

  /// <summary>Jump initial impulse force.</summary>
  [Export(PropertyHint.Range, "0, 100, 0.1")]
  public float JumpImpulseForce { get; set; } = 12f;

  /// <summary>
  ///   Additional force added each physics tick while player is still pressing
  ///   jump.
  /// </summary>
  [Export(PropertyHint.Range, "0, 100, 0.1")]
  public float JumpForce { get; set; } = 4.5f;

  #endregion Exports

  #region State

  public IPlayerLogic PlayerLogic { get; set; } = default!;
  public PlayerLogic.Settings Settings { get; set; } = default!;

  public Logic<PlayerLogic.IState, Func<object, PlayerLogic.IState>,
    PlayerLogic.IState, Action<PlayerLogic.IState?>>.IBinding PlayerBinding {
    get;
    set;
  } = default!;

  #endregion State

  public void Setup() {
    Settings = new PlayerLogic.Settings(
      RotationSpeed,
      StoppingSpeed,
      Gravity,
      MoveSpeed,
      Acceleration,
      JumpImpulseForce,
      JumpForce
    );

    PlayerLogic = new PlayerLogic(
      player: this,
      settings: Settings,
      appRepo: AppRepo,
      gameRepo: GameRepo
    );
  }

  public void OnReady() => SetPhysicsProcess(true);

  public void OnExitTree() {
    PlayerLogic.Stop();
    PlayerBinding.Dispose();
  }

  public void OnResolved() {
    Provide();

    PlayerBinding = PlayerLogic.Bind();

    GameRepo.SetPlayerGlobalPosition(GlobalPosition);

    PlayerBinding
      .Handle<PlayerLogic.Output.MovementComputed>(
        output => {
          Transform = Transform with { Basis = output.Rotation };
          Velocity = output.Velocity;
        }
      )
      .Handle<PlayerLogic.Output.VelocityChanged>(
        output => Velocity = output.Velocity
      );

    PlayerLogic.Start();
  }

  public void OnPhysicsProcess(double delta) {
    PlayerLogic.Input(new PlayerLogic.Input.PhysicsTick(delta));

    var jumpPressed = Input.IsActionPressed(GameInputs.Jump);
    var jumpJustPressed = Input.IsActionJustPressed(GameInputs.Jump);

    if (ShouldJump(jumpPressed, jumpJustPressed)) {
      PlayerLogic.Input(
        new PlayerLogic.Input.Jump(delta)
      );
    }

    MoveAndSlide();

    PlayerLogic.Input(new PlayerLogic.Input.Moved(GlobalPosition));
  }

  public static bool ShouldJump(bool jumpPressed, bool jumpJustPressed) =>
    jumpPressed || jumpJustPressed;

  #region IPlayer

  public Vector3 GetGlobalInputVector(Basis cameraBasis) {
    var rawInput = Input.GetVector(
      GameInputs.MoveLeft, GameInputs.MoveRight, GameInputs.MoveUp,
      GameInputs.MoveDown
    );
    // This is to ensure that diagonal input isn't stronger than axis aligned
    // input.
    var input = new Vector3 {
      X = rawInput.X * Mathf.Sqrt(1.0f - (rawInput.Y * rawInput.Y / 2.0f)),
      Z = rawInput.Y * Mathf.Sqrt(1.0f - (rawInput.X * rawInput.X / 2.0f))
    };
    return cameraBasis * input with { Y = 0f };
  }

  public Basis GetNextRotationBasis(
    Vector3 direction,
    double delta,
    float rotationSpeed
  ) {
    var leftAxis = Vector3.Up.Cross(direction);
    // Create a rotation quaternion from a basis with the 3 axis we care about.
    var rotationBasis =
      new Basis(leftAxis, Vector3.Up, direction).GetRotationQuaternion();
    var scale = Transform.Basis.Scale;
    // Let's lerp some quaternions so we rotate smoothly in the shortest
    // possible way.
    return new Basis(
      Transform
        .Basis
        .GetRotationQuaternion()
        .Slerp(rotationBasis, (float)delta * rotationSpeed)
    ).Scaled(scale);
  }

  [MethodImpl(
    MethodImplOptions.AggressiveInlining
  )]
  public bool IsMovingHorizontally() => (Velocity with { Y = 0f }).Length() >
                                        Settings.StoppingSpeed;

  #endregion IPlayer

  #region IPushEnabled

  public void Push(Vector3 force) =>
    PlayerLogic.Input(new PlayerLogic.Input.Pushed(force));

  #endregion IPushEnabled

  #region ICoinCollector

  public Vector3 CenterOfMass => GlobalPosition + new Vector3(0f, 1f, 0f);

  #endregion ICoinCollector

  #region IKillable

  public void Kill() => PlayerLogic.Input(new PlayerLogic.Input.Killed());

  #endregion IKillable
}
