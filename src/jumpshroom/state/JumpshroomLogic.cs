namespace GameDemo;

using System;
using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;

public interface IJumpshroomLogic : ILogicBlock;

[Meta]
public partial class JumpshroomLogic : LogicBlock, IJumpshroomLogic
{
  public override Type GetInitialState() => typeof(State.Idle);

  public JumpshroomLogic()
  {
    Set(new State.Cooldown());
    Set(new State.Idle());
    Set(new State.Launching());
    Set(new State.Loading());
  }
}
