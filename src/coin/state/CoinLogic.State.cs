namespace GameDemo;

using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;

public partial class CoinLogic
{
  [Meta, StateDiagram]
  public abstract partial record BaseState : LogicBlockState;
}
