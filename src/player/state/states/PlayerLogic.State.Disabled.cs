namespace GameDemo;

using System;
using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;

public abstract partial record PlayerLogicState
{
  [Meta, Id("player_logic_state_disabled")]
  public partial record Disabled : PlayerLogicState, IGet<Input.Enable>
  {
    public Disabled()
    {
      this.OnEnter(() => Output(new Output.Animations.Idle()));
    }

    public Type On(in Input.Enable input) => To<Idle>();

    public void OnGameEntered() => Input(new Input.Enable());
  }
}
