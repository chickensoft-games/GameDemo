namespace GameDemo;

using Chickensoft.LogicBlocks;
using Chickensoft.LogicBlocks.Generator;

public interface ICoinLogic : ILogicBlock<CoinLogic.IState> {
}

[StateDiagram(typeof(State))]
public partial class CoinLogic : LogicBlock<CoinLogic.IState>, ICoinLogic {
  public override IState GetInitialState() => new State.Idle();

  public record Settings(double CollectionTimeInSeconds);

  public CoinLogic(ICoin coin, Settings settings, IGameRepo gameRepo) {
    Set(coin);
    Set(settings);
    Set(gameRepo);
  }
}
