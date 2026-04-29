namespace GameDemo.Tests;

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Chickensoft.GoDotTest;
using Chickensoft.Sync.Primitives;
using Godot;
using Moq;
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
  private IGameRepo _gameRepo = default!;
  private IAppRepo _appRepo = default!;

  public GameLogicTest(Node testScene) : base(testScene) { }

  [Setup]
  public void Setup()
  {
    _logic = new GameLogic();
    _gameRepo = new GameRepo();
    _appRepo = new AppRepo();

    _logic.Set(_gameRepo);
    _logic.Set(_appRepo);
  }

  [Test]
  public void Initializes()
  {
    _logic
      .GetInitialState()
      .IsAssignableTo(typeof(GameLogic.BaseState)).ShouldBeTrue();
  }

  [Test]
  public void SubscribesToIsMouseCaptured()
  {
    var outputs = new List<object>();
    using var binding = _logic.Bind();

    binding.OnOutput(
      (in GameLogic.Output.CaptureMouse output) => outputs.Add(output)
    );

    _logic.Start();

    _gameRepo.IsMouseCaptured.Value.ShouldBe(false);

    _gameRepo.SetIsMouseCaptured(true);

    outputs.ShouldContain(new GameLogic.Output.CaptureMouse(true));
  }

  [Test]
  public void SubscribesToIsPaused()
  {
    var outputs = new List<object>();
    using var binding = _logic.Bind();

    binding.OnOutput(
      (in GameLogic.Output.SetPauseMode output) => outputs.Add(output)
    );

    _logic.Start();

    _gameRepo.IsPaused.Value.ShouldBe(false);

    _gameRepo.Pause();

    outputs.ShouldContain(new GameLogic.Output.SetPauseMode(true));
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
