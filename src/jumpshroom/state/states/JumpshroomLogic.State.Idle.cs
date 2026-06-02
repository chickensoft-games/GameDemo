namespace GameDemo;

using System;
using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;

public partial record JumpshroomLogicState
{
  [Meta]
  public partial record Idle : JumpshroomLogicState, IGet<Input.Hit>
  {
    public Type On(in Input.Hit input)
    {
      var target = input.Target;
      Get<JumpshroomLogic.Data>().Target = target;
      return To<Loading>();
    }
  }
}
