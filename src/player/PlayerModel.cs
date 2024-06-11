namespace GameDemo;

using Chickensoft.AutoInject;
using Chickensoft.GodotNodeInterfaces;
using Godot;
using Chickensoft.Introspection;

public interface IPlayerModel;

[Meta(typeof(IAutoNode))]
public partial class PlayerModel : Node3D {
  public override void _Notification(int what) => this.Notify(what);

  #region Dependencies
  [Dependency]
  public IPlayerLogic PlayerLogic => this.DependOn<IPlayerLogic>();
  #endregion Dependencies

  public PlayerLogic.IBinding PlayerBinding { get; set; } =
    default!;

  #region Nodes
  [Node("%AnimationTree")]
  public IAnimationTree AnimationTree { get; set; } = default!;
  public IAnimationNodeStateMachinePlayback AnimationStateMachine {
    get; set;
  } = default!;
  #endregion Nodes

  public void OnReady() => AnimationStateMachine =
    GodotInterfaces.Adapt<IAnimationNodeStateMachinePlayback>(
      (AnimationNodeStateMachinePlayback)AnimationTree.Get(
      "parameters/main_animations/playback"
      )
    );

  public void OnResolved() {
    PlayerBinding = PlayerLogic.Bind();

    PlayerBinding
      .Handle((in PlayerLogic.Output.Animations.Idle output) =>
        AnimationStateMachine.Travel("idle")
      )
      .Handle((in PlayerLogic.Output.Animations.Move output) =>
        AnimationStateMachine.Travel("move")
      )
      .Handle((in PlayerLogic.Output.Animations.Jump output) =>
        AnimationStateMachine.Travel("jump")
      )
      .Handle((in PlayerLogic.Output.Animations.Fall output) =>
        AnimationStateMachine.Travel("fall")
      )
      .Handle((in PlayerLogic.Output.MoveSpeedChanged output) =>
        AnimationTree.Set(
          "parameters/main_animations/move/blend_position", output.Speed
        )
      );
  }

  public void OnExitTree() => PlayerBinding.Dispose();
}
