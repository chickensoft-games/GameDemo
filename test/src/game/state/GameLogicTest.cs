namespace GameDemo.Tests;

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Chickensoft.GoDotTest;
using Godot;
using Shouldly;

[
  SuppressMessage(
    "Design",
    "CA1001",
    Justification = "Disposable field is disposed in cleanup"
  )
]
public class GameLogicTest : TestClass
{
  private GameLogic _logic = default!;
  private GameRepo _gameRepo = default!;
  private AppRepo _appRepo = default!;

  public GameLogicTest(Node testScene) : base(testScene) { }

  [Setup]
  public void Setup()
  {
    _logic = new GameLogic();
    _gameRepo = new GameRepo();
    _appRepo = new AppRepo();

    _logic.Set((IGameRepo)_gameRepo);
    _logic.Set((IAppRepo)_appRepo);
  }

  [Test]
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

  [Test]
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

  [Test]
  public void OnStopWithoutStartSucceeds()
  {
    var logic = new GameLogic();
    logic.OnStop();
  }

  [Cleanup]
  public void Cleanup()
  {
    _logic.Stop();
    _gameRepo.Dispose();
    _appRepo.Dispose();
  }
}
