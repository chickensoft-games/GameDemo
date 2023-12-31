namespace GameDemo;

public partial class AppLogic {
  public interface IState : IStateLogic {
  }

  public partial record State : StateLogic, IState {
  }
}
