namespace GameDemo;

using Chickensoft.LogicBlocks;

public partial class PlayerLogic {
  public interface IState : IStateLogic<IState>;

  public abstract partial record State : StateLogic<IState>, IState;
}
