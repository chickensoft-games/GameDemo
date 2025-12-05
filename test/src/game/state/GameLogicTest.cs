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
  private Mock<IGameRepo> _gameRepo = default!;
  private AutoValue<bool> _isMouseCaptured = default!;
  private AutoValue<bool> _isPaused = default!;

  public GameLogicTest(Node testScene) : base(testScene) { }

  [Setup]
  public void Setup()
  {
    _logic = new GameLogic();
    _gameRepo = new();
    _isMouseCaptured = new AutoValue<bool>(false);
    _isPaused = new AutoValue<bool>(false);

    _gameRepo.Setup(repo => repo.IsMouseCaptured).Returns(_isMouseCaptured);
    _gameRepo.Setup(repo => repo.IsPaused).Returns(_isPaused);

    _logic.Set(_gameRepo.Object);
  }

  [Test]
  public void Initializes()
  {
    _logic
      .GetInitialState().State
      .ShouldBeAssignableTo<GameLogic.State>();
  }

  [Test]
  public void SubscribesToIsMouseCaptured()
  {
    var outputs = new List<object>();
    using var binding = _logic.Bind();

    binding.Handle(
      (in GameLogic.Output.CaptureMouse output) => outputs.Add(output)
    );

    _logic.Start();

    _isMouseCaptured.Value.ShouldBe(false);

    _isMouseCaptured.Value = true;

    outputs.ShouldContain(new GameLogic.Output.CaptureMouse(true));
  }

  [Test]
  public void SubscribesToIsPaused()
  {
    var outputs = new List<object>();
    using var binding = _logic.Bind();

    binding.Handle(
      (in GameLogic.Output.SetPauseMode output) => outputs.Add(output)
    );

    _logic.Start();

    _isPaused.Value.ShouldBe(false);

    _isPaused.Value = true;

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

    _isMouseCaptured.Dispose();
    _isPaused.Dispose();
  }
}
