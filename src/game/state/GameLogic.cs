namespace GameDemo;

using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;

public interface IGameLogic : ILogicBlock<GameLogic.State>;

[Meta]
[LogicBlock(typeof(State), Diagram = true)]
public partial class GameLogic : LogicBlock<GameLogic.State>, IGameLogic {
  public override Transition GetInitialState() => To<State.MenuBackdrop>();
}
