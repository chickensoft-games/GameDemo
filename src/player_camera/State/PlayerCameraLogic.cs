namespace GameDemo;

using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;
using Chickensoft.Sync.Primitives;
using Godot;

public interface IPlayerCameraLogic : ILogicBlock<PlayerCameraLogic.State>;

[Meta, Id("player_camera_logic")]
[LogicBlock(typeof(State), Diagram = true)]
public partial class PlayerCameraLogic :
  LogicBlock<PlayerCameraLogic.State>, IPlayerCameraLogic
{
  private AutoValue<bool>.Binding? _isMouseCapturedBinding;

  private AutoValue<Vector3>.Binding? _playerGlobalPositionBinding;

  public override Transition GetInitialState() => To<State.InputDisabled>();

  public override void OnStart()
  {
    var gameRepo = Get<IGameRepo>();
    _isMouseCapturedBinding = gameRepo.IsMouseCaptured.Bind()
      .OnValue((isMouseCaptured) =>
      {
        if (isMouseCaptured)
        {
          Input(new Input.EnableInput());
          return;
        }

        Input(new Input.DisableInput());
      });

    _playerGlobalPositionBinding = gameRepo.PlayerGlobalPosition.Bind()
      .OnValue((playerGlobalPosition) => Input(new Input.TargetPositionChanged(playerGlobalPosition)));
  }

  public override void OnStop()
  {
    _isMouseCapturedBinding?.Dispose();
    _playerGlobalPositionBinding?.Dispose();
  }
}
