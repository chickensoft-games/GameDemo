namespace GameDemo.Tests;

using System.Linq;
using Chickensoft.GoDotTest;
using Chickensoft.LogicBlocks;
using Godot;
using Moq;
using Shouldly;

public class PausedTest : TestClass {
  private IFakeContext _context = default!;
  private GameLogic.State.Paused _state = default!;
  private Mock<IGameRepo> _gameRepo = default!;

  public PausedTest(Node testScene) : base(testScene) { }

  [Setup]
  public void Setup() {
    _state = new GameLogic.State.Paused();
    _context = _state.CreateFakeContext();

    _gameRepo = new();
    _context.Set(_gameRepo.Object);
  }

  [Test]
  public void OnEnter() {
    _gameRepo.Setup(repo => repo.Pause());
    _state.Enter();
    _context.Outputs.Single().ShouldBeOfType<GameLogic.Output.ShowPauseMenu>();
    _gameRepo.VerifyAll();
  }

  [Test]
  public void OnExit() {
    _state.Exit();
    _context.Outputs.Single().ShouldBeOfType<GameLogic.Output.ExitPauseMenu>();
  }

  [Test]
  public void OnPauseButtonPressed() {
    var result = _state.On(new GameLogic.Input.PauseButtonPressed());
    result.State.ShouldBeOfType<GameLogic.State.Resuming>();
  }

  [Test]
  public void OnGameSaveRequested() {
    var result = _state.On(new GameLogic.Input.SaveRequested());
    result.State.ShouldBeOfType<GameLogic.State.Saving>();
  }

  [Test]
  public void OnGoToMainMenu() {
    var result = _state.On(new GameLogic.Input.GoToMainMenu());
    result.State.ShouldBeOfType<GameLogic.State.Quit>();
  }
}
