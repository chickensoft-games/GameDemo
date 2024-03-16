namespace GameDemo;

using Chickensoft.LogicBlocks;

public partial class CoinLogic {
  public interface IState : IStateLogic<IState>;

  public abstract partial record State : StateLogic<IState>, IState;
}
