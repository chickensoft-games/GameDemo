namespace GameDemo;

using System;
using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;

public partial class CoinLogic
{
  public partial record BaseState
  {
    [Meta, Id("coin_logic_state_idle")]
    public partial record Idle : BaseState, IGet<Input.StartCollection>
    {
      public Type On(in Input.StartCollection input)
      {
        Get<Data>().Target = input.Target.Name;
        return To<Collecting>();
      }
    }
  }
}
