namespace GameDemo;

using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;

public partial class InGameUILogic
{
  [Meta]
  public partial record BaseState : LogicBlockState;
}
