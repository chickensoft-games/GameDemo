namespace GameDemo;

using System;
using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;

public partial class PlayerLogic
{
  public abstract partial record BaseState
  {
    [Meta, Id("player_logic_state_disabled")]
    public partial record Disabled : BaseState, IGet<Input.Enable>
    {
      public Disabled()
      {
        this.OnEnter(() => Output(new Output.Animations.Idle()));
      }

      public Type On(in Input.Enable input) => To<Idle>();
    }
  }
}
