namespace GameDemo;

public partial class AppLogic {
  public interface IState : IStateLogic {
  }

  public abstract partial record State : StateLogic, IState {
  }
}
