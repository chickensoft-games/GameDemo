namespace GameDemo;

using System;
using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;

public partial class PlayerLogic
{
  public abstract partial record BaseState
  {
    [Meta, Id("player_logic_state_alive_grounded_moving")]
    public partial record Moving : Grounded,
    IGet<Input.StoppedMovingHorizontally>
    {
      public Moving()
      {
        this.OnEnter(() => Output(new Output.Animations.Move()));
      }

      public Type On(in Input.StoppedMovingHorizontally input) =>
        To<Idle>();
    }
  }
}
