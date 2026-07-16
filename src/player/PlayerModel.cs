namespace GameDemo;

using Chickensoft.AutoInject;
using Chickensoft.GodotNodeInterfaces;
using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;
using Godot;

public interface IPlayerModel : INode3D;

[Meta(typeof(IAutoNode))]
public partial class PlayerModel : Node3D, IPlayerModel
{
  public override void _Notification(int what) => this.Notify(what);

  public const string ANIM_STATE_MACHINE = "parameters/StateMachine/playback";
  public const string BLINK_REQUEST = "parameters/BlinkShot/request";
  public const string LEAN_ADD = "parameters/LeanAdd/add_amount";
  public const string LEAN_DIRECTION_BLEND =
    "parameters/LeanDirectionBlend/blend_position";

  #region Dependencies
  [Dependency]
  public IPlayerLogic PlayerLogic => this.DependOn<IPlayerLogic>();
  [Dependency]
  public PlayerLogic.Settings Settings => this.DependOn<PlayerLogic.Settings>();
  #endregion Dependencies

  public LogicBlock.Binding PlayerBinding { get; set; } =
    default!;

  #region Nodes
  [Node("%AnimationTree")]
  public IAnimationTree AnimationTree { get; set; } = default!;
  public IAnimationNodeStateMachinePlayback AnimationStateMachine
  {
    get; set;
  } = default!;
  [Node("%VisualRoot")]
  public INode3D VisualRoot { get; set; } = default!;
  [Node("%CenterRoot")]
  public INode3D CenterRoot { get; set; } = default!;
  [Node("%BlinkTimer")]
  public ITimer BlinkTimer { get; set; } = default!;
  #endregion Nodes

  private float _lean;

  public void OnEnterTree() => BlinkTimer.Timeout += OnBlink;

  public void OnExitTree()
  {
    PlayerBinding.Dispose();
    BlinkTimer.Timeout -= OnBlink;
  }

  public void OnReady()
  {
    AnimationStateMachine = 
      GodotInterfaces.Adapt<IAnimationNodeStateMachinePlayback>(
        (AnimationNodeStateMachinePlayback)(GodotObject)AnimationTree.Get(
          ANIM_STATE_MACHINE
        )
      );
      )
    );
  }

  public void OnResolved()
  {
    PlayerBinding = PlayerLogic.Bind();

    PlayerBinding
      .OnOutput((in PlayerLogicState.Output.Animations.Idle output) =>
        AnimationStateMachine.Travel("idle")
      )
      .OnOutput((in PlayerLogicState.Output.Animations.Move output) =>
        AnimationStateMachine.Travel("run")
      )
      .OnOutput((in PlayerLogicState.Output.Animations.Jump output) =>
        AnimationStateMachine.Travel("jump")
      )
      .OnOutput((in PlayerLogicState.Output.Animations.Fall output) =>
        AnimationStateMachine.Travel("fall")
      )
      .OnOutput((in PlayerLogicState.Output.MoveSpeedChanged output) =>
        AnimationTree.Set(
          "parameters/main_animations/move/blend_position", output.Speed
        )
      )
      .OnOutput((in PlayerLogicState.Output.MovementComputed output) =>
      {
        var rotation = output.Rotation.GetEuler();
        var direction = output.Direction * -1;
        var targetAngle = Mathf.Atan2(direction.X, direction.Y);

        VisualRoot.Rotation = VisualRoot.Rotation with
        {
          Y = Mathf.RotateToward(
            VisualRoot.Rotation.Y,
            targetAngle,
            Settings.RotationSpeed * (float)output.Delta
          )
        };

        var angleDiff = Mathf.AngleDifference(VisualRoot.Rotation.Y, targetAngle);
        _lean = Mathf.MoveToward(
          _lean, GetTarget(angleDiff, PlayerLogic.State), 2f * (float)output.Delta
        );

        AnimationTree.Set(LEAN_ADD, Mathf.Abs(_lean));
        AnimationTree.Set(LEAN_DIRECTION_BLEND, _lean);
      });
  }

  public static float GetTarget(float angleDiff, LogicBlockState? state)
  {
    if (state is PlayerLogicState.Grounded)
    {
      return angleDiff;
    }

    return 0;
  }

  public void OnBlink() => AnimationTree.Set(BLINK_REQUEST, (int)AnimationNodeOneShot.OneShotRequest.Fire);
}
