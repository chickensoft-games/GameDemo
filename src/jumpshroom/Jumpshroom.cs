namespace GameDemo;

using Chickensoft.AutoInject;
using Chickensoft.GodotNodeInterfaces;
using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;
using Godot;

public interface IJumpshroom
{
  /// <summary>
  ///   Calling this informs the jumpshroom that something hit it.
  /// </summary>
  void Hit();
}

[Meta(typeof(IAutoNode))]
public partial class Jumpshroom : Node3D
{
  public override void _Notification(int what) => this.Notify(what);

  #region Signals

  [Signal]
  public delegate void ShroomLoadedEventHandler();

  #endregion Signals

  #region State

  public IJumpshroomLogic JumpshroomLogic { get; set; } = default!;

  public LogicBlock.Binding JumpshroomBinding { get; set; }
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

  [Dependency] public IGameRepo GameRepo => this.DependOn<IGameRepo>();

  #endregion Dependencies

  public void Setup()
  {
    JumpshroomLogic = new JumpshroomLogic();
    JumpshroomLogic.Set(new JumpshroomLogic.Data(ImpulseStrength));
  }

  public void OnResolved()
  {
    JumpshroomLogic.Set(GameRepo);

    JumpshroomBinding = JumpshroomLogic.Bind();

    ShroomLoaded += OnShroomLoaded;
    AnimationPlayer.AnimationFinished += OnAnimationFinished;
    Area3D.BodyEntered += OnAreaBodyEntered;
    CooldownTimer.Timeout += OnCooldownTimeout;

    JumpshroomBinding
      .OnOutput(
        (in JumpshroomLogicState.Output.Animate output) =>
          AnimationPlayer.Play("bounce")
      )
      .OnOutput(
        (in JumpshroomLogicState.Output.StartCooldownTimer output) =>
          CooldownTimer.Start()
      );

    JumpshroomLogic.Start<JumpshroomLogicState.Idle>();
  }

  public void OnCooldownTimeout() => JumpshroomLogic.Input(new JumpshroomLogicState.Input.CooldownCompleted());

  public void OnAreaBodyEntered(Node3D body)
  {
    if (body is IPushEnabled target)
    {
      // Whenever a push-enabled body comes into contact with us, we can
      // immediately start the launch process (if the state allows it).
      JumpshroomLogic.Input(new JumpshroomLogicState.Input.Hit(target));
    }
  }

  public void OnShroomLoaded() =>
    // We finished the windup part of the animation, now it's time to launch
    // whatever push-enabled object we are colliding with.
    JumpshroomLogic.Input(
      new JumpshroomLogicState.Input.Launch()
    );

  // Tell the state machine we finished animating so it can go back to idle.
  public void OnAnimationFinished(StringName animationName) =>
    JumpshroomLogic.Input(new JumpshroomLogicState.Input.LaunchCompleted());

  public void OnExitTree()
  {
    JumpshroomLogic.Stop();

    ShroomLoaded -= OnShroomLoaded;
    AnimationPlayer.AnimationFinished -= OnAnimationFinished;
    Area3D.BodyEntered -= OnAreaBodyEntered;
    CooldownTimer.Timeout -= OnCooldownTimeout;

    JumpshroomBinding.Dispose();
  }
}
