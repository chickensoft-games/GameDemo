namespace GameDemo;

using Chickensoft.LogicBlocks;
using Chickensoft.LogicBlocks.Generator;

public interface IInGameUILogic : ILogicBlock<InGameUILogic.IState> { }

// This state machine is nothing more than glue to the app repository.
// If the UI were more sophisticated, it'd be easy to expand on this.

[StateMachine]
public partial class InGameUILogic : LogicBlock<InGameUILogic.IState>, IInGameUILogic {
  public override IState GetInitialState() => new State();

  public InGameUILogic(IInGameUI inGameUi, IAppRepo appRepo) {
    Set(inGameUi);
    Set(appRepo);
  }
}
