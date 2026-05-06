namespace GameDemo;

using System;
using System.Collections.Generic;
using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;
using Chickensoft.Sync.Primitives;

public interface IGameLogic : ILogicBlock;

[Meta]
public partial class GameLogic : LogicBlock, IGameLogic
{
  private AutoValue<bool>.Binding? _isMouseCapturedBinding;
  private AutoValue<bool>.Binding? _isPausedBinding;

  public override Type GetInitialState() => typeof(BaseState.MenuBackdrop);

  public GameLogic()
  {
    Set(new BaseState.Lost());
    Set(new BaseState.MenuBackdrop());
    Set(new BaseState.Paused());
    Set(new BaseState.Saving());
    Set(new BaseState.Playing());
    Set(new BaseState.Quit());
    Set(new BaseState.RestartingGame());
    Set(new BaseState.Resuming());
    Set(new BaseState.Won());
  }

  public override IEnumerable<IDisposable> OnStartSubscriptions()
  {
    yield return Get<IAppRepo>().AutoChannel.Bind()
      .On((in IAppRepo.GameEntered _) => (State as BaseState.MenuBackdrop)?.OnGameEntered());
    yield return Get<IGameRepo>().AutoChannel.Bind()
      .On((in IGameRepo.Ended message) => (State as BaseState.Playing)?.OnEnded(message.Reason));
  }

  public override void OnStart()
  {
    var gameRepo = Get<IGameRepo>();
    _isMouseCapturedBinding = gameRepo.IsMouseCaptured.Bind()
      .OnValue((isMouseCaptured) => State?.Output(new Output.CaptureMouse(isMouseCaptured)));
    _isPausedBinding = gameRepo.IsPaused.Bind()
      .OnValue((isPaused) => State?.Output(new Output.SetPauseMode(isPaused)));
  }

  public override void OnStop()
  {
    _isMouseCapturedBinding?.Dispose();
    _isPausedBinding?.Dispose();
  }
}
