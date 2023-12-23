namespace GameDemo;

using Chickensoft.LogicBlocks;

public partial class JumpshroomLogic : LogicBlock<JumpshroomLogic.IState> {
  public interface IState : IStateLogic { }

  public abstract partial record State : StateLogic, IState;
}
