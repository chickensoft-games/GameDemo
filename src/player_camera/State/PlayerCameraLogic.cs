namespace GameDemo;

using Chickensoft.LogicBlocks;
using Chickensoft.LogicBlocks.Generator;
using Godot;

public interface IPlayerCameraLogic : ILogicBlock<PlayerCameraLogic.IState>;

[StateMachine]
public partial class PlayerCameraLogic :
  LogicBlock<PlayerCameraLogic.IState>, IPlayerCameraLogic {
  public override IState GetInitialState() => new State.InputDisabled();

  public PlayerCameraLogic(
    IPlayerCamera camera,
    PlayerCameraSettings settings,
    IGameRepo gameRepo
  ) {
    Set(camera);
    Set(settings);
    Set(gameRepo);

    Set(
      new Data {
        TargetPosition = Vector3.Zero,
        TargetAngleHorizontal = 0f,
        TargetAngleVertical = 0f,
        TargetOffset = Vector3.Zero
      }
    );
  }
}
