namespace GameDemo;

using System;
using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;

public partial class JumpshroomLogic
{
  public partial record BaseState
  {
    [Meta]
    public partial record Cooldown : BaseState, IGet<Input.CooldownCompleted>
    {
      public Cooldown()
      {
        this.OnEnter(() => Output(new Output.StartCooldownTimer()));
      }

      public Type On(in Input.CooldownCompleted input) => To<Idle>();
    }
  }
}
