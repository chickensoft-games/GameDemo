namespace GameDemo;

using System;
using System.Collections.Generic;
using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;
using Chickensoft.LogicBlocks.Auto;
using Chickensoft.Sync.Primitives;

public interface IGameLogic : ILogicBlock;

[Meta]
public partial class GameLogic : AutoBlock, IGameLogic
{
  private AutoValue<bool>.Binding? _isMouseCapturedBinding;
  private AutoValue<bool>.Binding? _isPausedBinding;

  public GameLogic()
  {
    Preallocate<GameLogicState>();
  }

  public override IEnumerable<IDisposable> OnStartSubscriptions()
  {
    yield return Get<IAppRepo>().AutoChannel.Bind()
      .On((in IAppRepo.GameEntered _) => (State as GameLogicState.MenuBackdrop)?.OnGameEntered());
    yield return Get<IGameRepo>().AutoChannel.Bind()
      .On((in IGameRepo.Ended message) => (State as GameLogicState.Playing)?.OnEnded(message.Reason));
  }

  public override void OnStart()
  {
    var gameRepo = Get<IGameRepo>();
    _isMouseCapturedBinding = gameRepo.IsMouseCaptured.Bind()
      .OnValue((isMouseCaptured) => State?.Output(new GameLogicState.Output.CaptureMouse(isMouseCaptured)));
    _isPausedBinding = gameRepo.IsPaused.Bind()
      .OnValue((isPaused) => State?.Output(new GameLogicState.Output.SetPauseMode(isPaused)));
  }

  public override void OnStop()
  {
    _isMouseCapturedBinding?.Dispose();
    _isPausedBinding?.Dispose();
  }
}
