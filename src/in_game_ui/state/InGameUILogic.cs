namespace GameDemo;

using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;

public interface IInGameUILogic : ILogicBlock<InGameUILogic.State>;

// This state machine is nothing more than glue to the app repository.
// If the UI were more sophisticated, it'd be easy to expand on this.

[Meta]
[LogicBlock(typeof(State))]
public partial class InGameUILogic : LogicBlock<InGameUILogic.State>, IInGameUILogic
{
  public override Transition GetInitialState() => To<State>();
}
