namespace GameDemo;

using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;
using Chickensoft.Sync.Primitives;

public interface IInGameUILogic : ILogicBlock<InGameUILogic.State>;

// This state machine is nothing more than glue to the game repository.
// If the UI were more sophisticated, it'd be easy to expand on this.

[Meta]
[LogicBlock(typeof(State))]
public partial class InGameUILogic : LogicBlock<InGameUILogic.State>, IInGameUILogic
{
  private AutoValue<int>.Binding? _numCoinsCollectedBinding;
  private AutoValue<int>.Binding? _numCoinsAtStartBinding;

  public override Transition GetInitialState() => To<State>();

  public override void OnStart()
  {
    var gameRepo = Get<IGameRepo>();
    _numCoinsCollectedBinding = gameRepo.NumCoinsCollected.Bind()
      .OnValue((numCoinsCollected) => Context.Output(new Output.NumCoinsChanged(numCoinsCollected, gameRepo.NumCoinsAtStart.Value)));
    _numCoinsAtStartBinding = gameRepo.NumCoinsAtStart.Bind()
      .OnValue((numCoinsAtStart) => Context.Output(new Output.NumCoinsChanged(gameRepo.NumCoinsCollected.Value, numCoinsAtStart)));
  }

  public override void OnStop()
  {
    _numCoinsCollectedBinding?.Dispose();
    _numCoinsAtStartBinding?.Dispose();
  }
}
