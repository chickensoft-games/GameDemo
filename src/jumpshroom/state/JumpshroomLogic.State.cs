namespace GameDemo;

using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;

public partial class JumpshroomLogic
{
  [Meta]

  public abstract partial record BaseState : LogicBlockState;
}
