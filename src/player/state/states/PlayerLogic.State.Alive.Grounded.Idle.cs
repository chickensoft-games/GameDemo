namespace GameDemo;

using System;
using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;

public partial class PlayerLogic
{
  public abstract partial record BaseState
  {
    [Meta, Id("player_logic_state_alive_grounded_idle")]
    public partial record Idle : Grounded,
    IGet<Input.StartedMovingHorizontally>
    {
      public Idle()
      {
        this.OnEnter(() => Output(new Output.Animations.Idle()));
      }

      public Type On(in Input.StartedMovingHorizontally input) =>
        To<Moving>();
    }
  }
}
