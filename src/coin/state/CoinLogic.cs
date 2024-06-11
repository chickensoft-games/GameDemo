namespace GameDemo;

using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;

public interface ICoinLogic : ILogicBlock<CoinLogic.State>;

[Meta, Id("coin_logic")]
[LogicBlock(typeof(State), Diagram = true)]
public partial class CoinLogic : LogicBlock<CoinLogic.State>, ICoinLogic {
  public override Transition GetInitialState() => To<State.Idle>();

  public record Settings(double CollectionTimeInSeconds);
}
