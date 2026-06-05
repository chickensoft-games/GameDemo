namespace GameDemo;

using System;
using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;
using Chickensoft.LogicBlocks.Auto;

public interface IJumpshroomLogic : ILogicBlock;

[Meta]
public partial class JumpshroomLogic : AutoBlock, IJumpshroomLogic
{
  public JumpshroomLogic()
  {
    Preallocate<JumpshroomLogicState>();
  }
}
