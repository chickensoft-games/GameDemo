namespace GameDemo;

using Chickensoft.LogicBlocks;
using Chickensoft.LogicBlocks.Generator;

public interface IPlayerLogic : ILogicBlock<PlayerLogic.IState> { }

[StateDiagram(typeof(State))]
public partial class PlayerLogic : LogicBlock<PlayerLogic.IState>, IPlayerLogic {
  public override IState GetInitialState() => new State.Disabled();

  public PlayerLogic(
    IPlayer player, Settings settings, IAppRepo appRepo, IGameRepo gameRepo
  ) {
    Set(player);
    Set(settings);
    Set(appRepo);
    Set(gameRepo);
    Set(new Data());
  }
}
