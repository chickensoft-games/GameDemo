namespace GameDemo;

public partial class CoinLogic {
  public interface IState : IStateLogic { }

  public abstract partial record State(IContext Context) :
    StateLogic(Context), IState;
}
