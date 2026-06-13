namespace GameDemo;

using System;
using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;

public partial record JumpshroomLogicState
{
  [Meta]
  public partial record Cooldown : JumpshroomLogicState, IGet<Input.CooldownCompleted>
  {
    public Cooldown()
    {
      this.OnEnter(() => Output(new Output.StartCooldownTimer()));
    }

    public Type On(in Input.CooldownCompleted input) => To<Idle>();
  }
}
