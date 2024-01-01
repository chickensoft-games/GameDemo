namespace GameDemo;

public partial class GameLogic {
  public interface IState : IStateLogic {
  }

  public abstract partial record State : StateLogic, IState {
  }
}
