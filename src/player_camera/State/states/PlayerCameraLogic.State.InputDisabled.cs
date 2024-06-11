namespace GameDemo;

using Chickensoft.Introspection;

public partial class PlayerCameraLogic {
  public partial record State {
    [Meta, Id("player_camera_logic_state_input_disabled")]
    public partial record InputDisabled : State, IGet<Input.EnableInput> {
      public Transition On(in Input.EnableInput input) => To<InputEnabled>();
    }
  }
}
