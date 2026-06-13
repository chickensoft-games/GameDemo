namespace GameDemo;

using System;
using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;

public partial record CoinLogicState
{
  [Meta, Id("coin_logic_state_idle")]
  public partial record Idle : CoinLogicState, IGet<Input.StartCollection>
  {
    public Type On(in Input.StartCollection input)
    {
      Get<CoinLogic.Data>().Target = input.Target.Name;
      return To<Collecting>();
    }
  }
}
