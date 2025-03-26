namespace GameDemo;

using Chickensoft.AutoInject;
using Chickensoft.Collections;
using Chickensoft.GodotNodeInterfaces;
using Chickensoft.Introspection;
using Chickensoft.SaveFileBuilder;
using Godot;
using Compiler = System.Runtime.CompilerServices;

public interface IPlayer :
  ICharacterBody3D, IKillable, ICoinCollector, IPushEnabled {
  IPlayerLogic PlayerLogic { get; }

  bool IsMovingHorizontally();

  /// <summary>
  ///   Uses the engine to determine the input vector, relative to the global
  ///   camera direction.
  /// </summary>
  /// <param name="cameraBasis">Camera's global transform basis.</param>
  Vector3 GetGlobalInputVector(Basis cameraBasis);

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

[Meta(typeof(IAutoNode))]
public partial class Player :
CharacterBody3D,
IPlayer,
IProvide<IPlayerLogic>,
IProvide<PlayerLogic.Settings> {
  public override void _Notification(int what) => this.Notify(what);

  #region Save

  [Dependency]
  public EntityTable EntityTable => this.DependOn<EntityTable>();
  [Dependency]
  public ISaveChunk<GameData> GameChunk => this.DependOn<ISaveChunk<GameData>>();
  public ISaveChunk<PlayerData> PlayerChunk { get; set; } = default!;
  #endregion Save

  #region Provisions

  IPlayerLogic IProvide<IPlayerLogic>.Value() => PlayerLogic;
  PlayerLogic.Settings IProvide<PlayerLogic.Settings>.Value() => Settings;
  #endregion Provisions

  #region Dependencies

  [Dependency]
  public IGameRepo GameRepo => this.DependOn<IGameRepo>();

  [Dependency]
  public IAppRepo AppRepo => this.DependOn<IAppRepo>();

  #endregion Dependencies

  #region Exports

  /// <summary>Rotation speed (quaternions?/sec).</summary>
  [Export(PropertyHint.Range, "0, 100, 0.1")]
  public float RotationSpeed { get; set; } = 4.0f;

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

  public PlayerLogic.IBinding PlayerBinding { get; set; } = default!;

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

    PlayerLogic = new PlayerLogic();

    PlayerLogic.Set(this as IPlayer);
    PlayerLogic.Set(Settings);
    PlayerLogic.Set(AppRepo);
    PlayerLogic.Set(GameRepo);
    PlayerLogic.Save(() => new PlayerLogic.Data());

    PlayerChunk = new SaveChunk<PlayerData>(
      onSave: (chunk) => new PlayerData() {
        GlobalTransform = GlobalTransform,
        StateMachine = PlayerLogic,
        Velocity = Velocity
      },
      onLoad: (chunk, data) => {
        GlobalTransform = data.GlobalTransform;
        Velocity = data.Velocity;
        PlayerLogic.RestoreFrom(data.StateMachine);
        PlayerLogic.Start();
      }
    );
  }

  public void OnReady() => SetPhysicsProcess(true);

  public void OnExitTree() {
    EntityTable.Remove(Name);
    PlayerLogic.Stop();
    PlayerBinding.Dispose();
  }

  public void OnResolved() {
    // Add a child to our parent save chunk (the game chunk) so that it can
    // look up the player chunk when loading and saving the game.
    GameChunk.AddChunk(PlayerChunk);

    EntityTable.Set(Name, this);

    PlayerBinding = PlayerLogic.Bind();

    GameRepo.SetPlayerGlobalPosition(GlobalPosition);

    PlayerBinding
      .Handle((in PlayerLogic.Output.MovementComputed output) =>
        Velocity = output.Velocity)
      .Handle((in PlayerLogic.Output.VelocityChanged output) =>
        Velocity = output.Velocity
      );

    // Allow the player model to lookup our state machine and bind to it.
    this.Provide();

    // Start the player state machine last.
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

  [Compiler.MethodImpl(Compiler.MethodImplOptions.AggressiveInlining)]
  public bool IsMovingHorizontally() =>
    (Velocity with { Y = 0f }).Length() > Settings.StoppingSpeed;

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
