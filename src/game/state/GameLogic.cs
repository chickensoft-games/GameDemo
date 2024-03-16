namespace GameDemo;

using Chickensoft.LogicBlocks;
using Chickensoft.LogicBlocks.Generator;

public interface IGameLogic : ILogicBlock<GameLogic.IState> {
}

[StateDiagram(typeof(State))]
public partial class GameLogic : LogicBlock<GameLogic.IState>, IGameLogic {
  public override IState GetInitialState() => new State.MenuBackdrop();

  public GameLogic(IGameRepo gameRepo, IAppRepo appRepo) {
    Set(gameRepo);
    Set(appRepo);
  }
}
