namespace GameDemo;

using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;
using Chickensoft.Sync.Primitives;

public interface IGameLogic : ILogicBlock<GameLogic.State>;

[Meta]
[LogicBlock(typeof(State), Diagram = true)]
public partial class GameLogic : LogicBlock<GameLogic.State>, IGameLogic
{
  private AutoValue<bool>.Binding? _isMouseCapturedBinding;
  private AutoValue<bool>.Binding? _isPausedBinding;

  public override Transition GetInitialState() => To<State.MenuBackdrop>();

  public override void OnStart()
  {
    var gameRepo = Get<IGameRepo>();
    _isMouseCapturedBinding = gameRepo.IsMouseCaptured.Bind()
      .OnValue((isMouseCaptured) => Context.Output(new Output.CaptureMouse(isMouseCaptured)));
    _isPausedBinding = gameRepo.IsPaused.Bind()
      .OnValue((isPaused) => Context.Output(new Output.SetPauseMode(isPaused)));
  }

  public override void OnStop()
  {
    _isMouseCapturedBinding?.Dispose();
    _isPausedBinding?.Dispose();
  }
}
