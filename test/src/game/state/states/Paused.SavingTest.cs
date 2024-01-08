namespace GameDemo.Tests;

using System.Linq;
using Chickensoft.GoDotCollections;
using Chickensoft.GoDotTest;
using Chickensoft.LogicBlocks;
using Godot;
using Moq;
using Shouldly;

public class PausedSavingTest : TestClass {
  private IFakeContext _context = default!;
  private GameLogic.State.Saving _state = default!;
  private Mock<IGameRepo> _gameRepo = default!;
  private AutoProp<bool> _isMouseCaptured = default!;
  private AutoProp<bool> _isPaused = default!;

  public PausedSavingTest(Node testScene) : base(testScene) { }

  [Setup]
  public void Setup() {
    _state = new GameLogic.State.Saving();
    _context = _state.CreateFakeContext();

    _isMouseCaptured = new(true);
    _isPaused = new(true);

    _gameRepo = new();
    _gameRepo.Setup(repo => repo.IsPaused).Returns(_isPaused);
    _gameRepo.Setup(repo => repo.IsMouseCaptured).Returns(_isMouseCaptured);

    _context.Set(_gameRepo.Object);
  }

  [Test]
  public void OnEnter() {
    _gameRepo.Reset();
    _gameRepo.Setup(repo => repo.Save());
    _state.Enter(new GameLogic.State.Paused());
    _context.Outputs.Single().ShouldBeOfType<GameLogic.Output.ShowPauseSaveOverlay>();
    _gameRepo.VerifyAll();
  }

  [Test]
  public void OnExit() {
    _state.Exit(new GameLogic.State.Paused());
    _context.Outputs.Single().ShouldBeOfType<GameLogic.Output.HidePauseSaveOverlay>();
  }

  [Test]
  public void Subscribes() {
    _state.Attach(_context);

    _gameRepo.VerifyAdd(repo => repo.SaveCompleted += _state.OnSaveCompleted);

    _state.Detach();

    _gameRepo.VerifyRemove(repo => repo.SaveCompleted -= _state.OnSaveCompleted);
  }

  [Test]
  public void OnSaveCompleted() {
    _state.OnSaveCompleted();
    _context.Inputs.Single().ShouldBeOfType<GameLogic.Input.SaveCompleted>();
  }

  [Test]
  public void OnInputSaveCompleted() =>
    _state.On(new GameLogic.Input.SaveCompleted())
      .ShouldBeOfType<GameLogic.State.Paused>();

  [Test]
  public void OnPauseButtonPressed() =>
    _state.On(new GameLogic.Input.PauseButtonPressed())
      .ShouldBe(_state);
}
