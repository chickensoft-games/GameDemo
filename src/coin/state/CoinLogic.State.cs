namespace GameDemo;

using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;

public partial class CoinLogic {
  [Meta]
  public abstract partial record State : StateLogic<State>;
}
