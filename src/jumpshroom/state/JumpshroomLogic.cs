namespace GameDemo;

using Chickensoft.LogicBlocks;
using Chickensoft.LogicBlocks.Generator;

public interface IJumpshroomLogic : ILogicBlock<JumpshroomLogic.IState> { }

[StateMachine]
public partial class JumpshroomLogic : LogicBlock<JumpshroomLogic.IState>, IJumpshroomLogic {
  public override State GetInitialState(IContext context) =>
    new State.Idle(context);

  public JumpshroomLogic(Data data, IAppRepo appRepo) {
    Set(data);
    Set(appRepo);
  }
}
