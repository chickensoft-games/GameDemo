namespace GameDemo;

public partial class PlayerCameraLogic {
  public partial record State {
    public record InputDisabled(IContext Context) : State(Context),
      IGet<Input.EnableInput> {
      public IState On(Input.EnableInput input) => new InputEnabled(Context);
    }
  }
}
