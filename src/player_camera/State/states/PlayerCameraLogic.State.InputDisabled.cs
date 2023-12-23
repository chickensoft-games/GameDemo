namespace GameDemo;

public partial class PlayerCameraLogic {
  public partial record State {
    public record InputDisabled : State, IGet<Input.EnableInput> {
      public IState On(Input.EnableInput input) => new InputEnabled();
    }
  }
}
