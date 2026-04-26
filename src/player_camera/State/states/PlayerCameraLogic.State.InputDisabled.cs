namespace GameDemo;

using System;
using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;

public partial class PlayerCameraLogic
{
  public partial record BaseState
  {
    [Meta, Id("player_camera_logic_state_input_disabled")]
    public partial record InputDisabled : BaseState, IGet<Input.EnableInput>
    {
      public Type On(in Input.EnableInput input) => To<InputEnabled>();
    }
  }
}
