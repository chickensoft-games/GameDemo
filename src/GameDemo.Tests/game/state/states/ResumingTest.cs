namespace GameDemo.Tests;

using System.Linq;
using Moq;
using Shouldly;

public class ResumingTest
{
  private readonly StateTester _context;
  private readonly GameLogicState.Resuming _state = new();
  private readonly Mock<IGameRepo> _gameRepo = new();

  public ResumingTest()
  {
    _context = _state.Test();

    _context.Set(_gameRepo.Object);
  }

  [Fact]
  public void OnEnter()
  {
    _gameRepo.Setup(repo => repo.Resume());
    _state.Enter();
    _gameRepo.VerifyAll();
  }

  [Fact]
  public void OnExit()
  {
    _state.Exit();
    _context.Outputs.Single().ShouldBeOfType<GameLogicState.Output.HidePauseMenu>();
  }

  [Fact]
  public void OnPauseMenuTransitioned()
  {
    var result = _state.On(new GameLogicState.Input.PauseMenuTransitioned());
    result.ShouldBe(typeof(GameLogicState.Playing));
  }
}
