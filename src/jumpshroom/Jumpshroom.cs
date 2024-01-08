namespace GameDemo;

using Chickensoft.AutoInject;
using Chickensoft.GodotNodeInterfaces;
using Chickensoft.PowerUps;
using Godot;
using SuperNodes.Types;

public interface IJumpshroom {
  /// <summary>
  ///   Calling this informs the jumpshroom that something hit it.
  /// </summary>
  public void Hit();
}

[SuperNode(typeof(AutoNode), typeof(Dependent))]
public partial class Jumpshroom : Node3D {
  public override partial void _Notification(int what);

  #region Signals

  [Signal]
  public delegate void ShroomLoadedEventHandler();

  #endregion Signals

  #region State

  public IJumpshroomLogic JumpshroomLogic { get; set; } = default!;

  public JumpshroomLogic.IBinding JumpshroomBinding { get; set; }
    = default!;

  [Export(PropertyHint.Range, "1,100,0.5")]
  public float ImpulseStrength { get; set; } = 30;

  #endregion State

  #region Nodes

  [Node("%AnimationPlayer")] public IAnimationPlayer AnimationPlayer { get; set; } = default!;
  [Node("%Area3D")] public IArea3D Area3D { get; set; } = default!;
  [Node("%Timer")] public ITimer CooldownTimer { get; set; } = default!;

  #endregion Nodes

  #region Dependencies

  [Dependency] public IGameRepo GameRepo => DependOn<IGameRepo>();

  #endregion Dependencies

  public void Setup() =>
    JumpshroomLogic = new JumpshroomLogic(
      new JumpshroomLogic.Data(ImpulseStrength), GameRepo
    );

  public void OnResolved() {
    JumpshroomBinding = JumpshroomLogic.Bind();

    ShroomLoaded += OnShroomLoaded;
    AnimationPlayer.AnimationFinished += OnAnimationFinished;
    Area3D.BodyEntered += OnAreaBodyEntered;
    CooldownTimer.Timeout += OnCooldownTimeout;

    JumpshroomBinding
      .Handle<JumpshroomLogic.Output.Animate>(
        output => AnimationPlayer.Play("bounce")
      )
      .Handle<JumpshroomLogic.Output.StartCooldownTimer>(
        output => CooldownTimer.Start()
      );
  }

  public void OnCooldownTimeout() => JumpshroomLogic.Input(new JumpshroomLogic.Input.CooldownCompleted());

  public void OnAreaBodyEntered(Node3D body) {
    if (body is IPushEnabled target) {
      // Whenever a push-enabled body comes into contact with us, we can
      // immediately start the launch process (if the state allows it).
      JumpshroomLogic.Input(new JumpshroomLogic.Input.Hit(target));
    }
  }

  public void OnShroomLoaded() =>
    // We finished the windup part of the animation, now it's time to launch
    // whatever push-enabled object we are colliding with.
    JumpshroomLogic.Input(
      new JumpshroomLogic.Input.Launch()
    );

  // Tell the state machine we finished animating so it can go back to idle.
  public void OnAnimationFinished(StringName animationName) =>
    JumpshroomLogic.Input(new JumpshroomLogic.Input.LaunchCompleted());

  public void OnExitTree() {
    JumpshroomLogic.Stop();

    ShroomLoaded -= OnShroomLoaded;
    AnimationPlayer.AnimationFinished -= OnAnimationFinished;
    Area3D.BodyEntered -= OnAreaBodyEntered;
    CooldownTimer.Timeout -= OnCooldownTimeout;

    JumpshroomBinding.Dispose();
  }
}
