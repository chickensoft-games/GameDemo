namespace GameDemo.Tests;

using System.Linq;
using Moq;
using Shouldly;

public class PausedTest
{
  private readonly StateTester _context;
  private readonly GameLogicState.Paused _state = new();
  private readonly Mock<IGameRepo> _gameRepo = new();

  public PausedTest()
  {
    _context = _state.Test();

    _context.Set(_gameRepo.Object);
  }

  [Fact]
  public void OnEnter()
  {
    _gameRepo.Setup(repo => repo.Pause());
    _state.Enter();
    _context.Outputs.Single().ShouldBeOfType<GameLogicState.Output.ShowPauseMenu>();
    _gameRepo.VerifyAll();
  }

  [Fact]
  public void OnExit()
  {
    _state.Exit();
    _context.Outputs.Single().ShouldBeOfType<GameLogicState.Output.ExitPauseMenu>();
  }

  [Fact]
  public void OnPauseButtonPressed()
  {
    var result = _state.On(new GameLogicState.Input.PauseButtonPressed());
    result.ShouldBe(typeof(GameLogicState.Resuming));
  }

  [Fact]
  public void OnGameSaveRequested()
  {
    var result = _state.On(new GameLogicState.Input.SaveRequested());
    result.ShouldBe(typeof(GameLogicState.Saving));
  }

  [Fact]
  public void OnGoToMainMenu()
  {
    var result = _state.On(new GameLogicState.Input.GoToMainMenu());
    result.ShouldBe(typeof(GameLogicState.Quit));
  }
}
