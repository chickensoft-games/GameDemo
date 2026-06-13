namespace GameDemo;

using System;
using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;

public partial record PlayerCameraLogicState
{
  [Meta, Id("player_camera_logic_state_input_disabled")]
  public partial record InputDisabled : PlayerCameraLogicState, IGet<Input.EnableInput>
  {
    public Type On(in Input.EnableInput input) => To<InputEnabled>();
  }
}
