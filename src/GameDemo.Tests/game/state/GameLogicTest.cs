namespace GameDemo.Tests;

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Shouldly;

[
  SuppressMessage(
    "Design",
    "CA1001",
    Justification = "Disposable field is disposed in cleanup"
  )
]
public class GameLogicTest : IDisposable
{
  private readonly GameLogic _logic = new();
  private readonly GameRepo _gameRepo = new();
  private readonly AppRepo _appRepo = new();

  public GameLogicTest()
  {
    _logic.Set<IGameRepo>(_gameRepo);
    _logic.Set<IAppRepo>(_appRepo);
  }

  public void Dispose()
  {
    _logic.Stop();
    _gameRepo.Dispose();
    _appRepo.Dispose();
    GC.SuppressFinalize(this);
  }

  [Fact]
  public void SubscribesToIsMouseCaptured()
  {
    var outputs = new List<object>();
    using var binding = _logic.Bind();

    binding.OnOutput(
      (in GameLogicState.Output.CaptureMouse output) => outputs.Add(output)
    );

    _logic.Start<GameLogicState.MenuBackdrop>();

    _gameRepo.IsMouseCaptured.Value.ShouldBe(false);

    _gameRepo.SetIsMouseCaptured(true);

    outputs.ShouldContain(new GameLogicState.Output.CaptureMouse(true));
  }

  [Fact]
  public void SubscribesToIsPaused()
  {
    var outputs = new List<object>();
    using var binding = _logic.Bind();

    binding.OnOutput(
      (in GameLogicState.Output.SetPauseMode output) => outputs.Add(output)
    );

    _logic.Start<GameLogicState.MenuBackdrop>();

    _gameRepo.IsPaused.Value.ShouldBe(false);

    _gameRepo.Pause();

    outputs.ShouldContain(new GameLogicState.Output.SetPauseMode(true));
  }

  [Fact]
  public void OnStopWithoutStartSucceeds()
  {
    var logic = new GameLogic();
    logic.OnStop();
  }
}
