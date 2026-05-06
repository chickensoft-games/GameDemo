namespace GameDemo;

using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;

public partial class GameLogic
{
  [Meta, StateDiagram]
  public abstract partial record BaseState : LogicBlockState;
}
