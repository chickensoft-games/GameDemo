namespace GameDemo;

using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;

public partial class GameLogic
{
  [Meta]
  public abstract partial record State : StateLogic<State>;
}
