namespace GameDemo;

using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;

public interface IMapLogic : ILogicBlock<MapLogic.State> { }

[Meta]
[LogicBlock(typeof(State))]
public partial class MapLogic : LogicBlock<MapLogic.State>, IMapLogic {
  public override Transition GetInitialState() => To<State>();
}
