namespace GameDemo.Tests;

using System.Linq;
using Chickensoft.GoDotTest;
using Chickensoft.LogicBlocks;
using Godot;
using Moq;
using Shouldly;

public class PausedTest : TestClass
{
  private StateTester _context = default!;
  private GameLogic.BaseState.Paused _state = default!;
  private Mock<IGameRepo> _gameRepo = default!;

  public PausedTest(Node testScene) : base(testScene) { }

  [Setup]
  public void Setup()
  {
    _state = new GameLogic.BaseState.Paused();
    _context = _state.Test();

    _gameRepo = new();
    _context.Set(_gameRepo.Object);
  }

  [Test]
  public void OnEnter()
  {
    _gameRepo.Setup(repo => repo.Pause());
    _state.Enter();
    _context.Outputs.Single().ShouldBeOfType<GameLogic.Output.ShowPauseMenu>();
    _gameRepo.VerifyAll();
  }

  [Test]
  public void OnExit()
  {
    _state.Exit();
    _context.Outputs.Single().ShouldBeOfType<GameLogic.Output.ExitPauseMenu>();
  }

  [Test]
  public void OnPauseButtonPressed()
  {
    var result = _state.On(new GameLogic.Input.PauseButtonPressed());
    result.ShouldBe(typeof(GameLogic.BaseState.Resuming));
  }

  [Test]
  public void OnGameSaveRequested()
  {
    var result = _state.On(new GameLogic.Input.SaveRequested());
    result.ShouldBe(typeof(GameLogic.BaseState.Saving));
  }

  [Test]
  public void OnGoToMainMenu()
  {
    var result = _state.On(new GameLogic.Input.GoToMainMenu());
    result.ShouldBe(typeof(GameLogic.BaseState.Quit));
  }
}
