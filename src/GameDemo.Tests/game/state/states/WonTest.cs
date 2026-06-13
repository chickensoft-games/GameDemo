namespace GameDemo.Tests;

using System.Linq;
using Moq;
using Shouldly;

public class WonTest
{
  private readonly StateTester _context;
  private readonly GameLogicState.Won _state = new();
  private readonly Mock<IAppRepo> _appRepo = new();

  public WonTest()
  {
    _context = _state.Test();

    _context.Set(_appRepo.Object);
  }

  [Fact]
  public void OnEnter()
  {
    _state.Enter();
    _context.Outputs.First().ShouldBeOfType<GameLogicState.Output.ShowWonScreen>();
  }

  [Fact]
  public void OnGoToMainMenu()
  {
    _appRepo.Setup(repo => repo.OnExitGame(PostGameAction.GoToMainMenu));

    var result = _state.On(new GameLogicState.Input.GoToMainMenu());

    _appRepo.VerifyAll();
    result.ShouldBe(_state.GetType());
  }
}
