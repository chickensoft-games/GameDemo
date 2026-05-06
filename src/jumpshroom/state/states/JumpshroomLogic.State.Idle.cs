namespace GameDemo;

using System;
using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;

public partial class JumpshroomLogic
{
  public partial record BaseState
  {
    [Meta]
    public partial record Idle : BaseState, IGet<Input.Hit>
    {
      public Type On(in Input.Hit input)
      {
        var target = input.Target;
        Get<Data>().Target = target;
        return To<Loading>();
      }
    }
  }
}
