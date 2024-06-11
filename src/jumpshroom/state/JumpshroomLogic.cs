namespace GameDemo;

using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;

public interface IJumpshroomLogic : ILogicBlock<JumpshroomLogic.State>;

[Meta]
[LogicBlock(typeof(State), Diagram = true)]
public partial class JumpshroomLogic : LogicBlock<JumpshroomLogic.State>, IJumpshroomLogic {
  public override Transition GetInitialState() => To<State.Idle>();
}
