namespace GameDemo;

using Chickensoft.LogicBlocks;

public partial class JumpshroomLogic : LogicBlock<JumpshroomLogic.IState> {
  public interface IState : IStateLogic<IState>;

  public abstract partial record State : StateLogic<IState>, IState;
}
