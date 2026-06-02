namespace GameDemo;

using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;

[Meta, StateDiagram]
public partial record InGameUILogicState : LogicBlockState
{
  public static class Output
  {
    public readonly record struct NumCoinsChanged(
      int NumCoinsCollected, int NumCoinsAtStart
    );
  }
}
