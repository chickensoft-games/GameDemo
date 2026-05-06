namespace GameDemo;

using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;

public partial class JumpshroomLogic
{
  [Meta, StateDiagram]
  public abstract partial record BaseState : LogicBlockState;
}
