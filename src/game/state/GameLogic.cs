namespace GameDemo;

using Chickensoft.LogicBlocks;
using Chickensoft.LogicBlocks.Generator;

public interface IGameLogic : ILogicBlock<GameLogic.IState> {
}

[StateMachine]
public partial class GameLogic : LogicBlock<GameLogic.IState>, IGameLogic {
  public override IState GetInitialState() => new State.InGame();

  public GameLogic(IGameRepo gameRepo, IAppRepo appRepo) {
    Set(gameRepo);
    Set(appRepo);
  }
}
