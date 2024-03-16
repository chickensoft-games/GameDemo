namespace GameDemo;

using Chickensoft.LogicBlocks;
using Chickensoft.LogicBlocks.Generator;

public interface IJumpshroomLogic : ILogicBlock<JumpshroomLogic.IState> {
}

[StateDiagram(typeof(State))]
public partial class JumpshroomLogic : LogicBlock<JumpshroomLogic.IState>, IJumpshroomLogic {
  public override State GetInitialState() => new State.Idle();

  public JumpshroomLogic(Data data, IGameRepo gameRepo) {
    Set(data);
    Set(gameRepo);
  }
}
