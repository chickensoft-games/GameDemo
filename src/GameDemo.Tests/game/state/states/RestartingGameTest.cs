namespace GameDemo.Tests;

using Moq;

public class RestartingGameTest
{
  private readonly StateTester _context;
  private readonly GameLogicState.RestartingGame _state = new();
  private readonly Mock<IAppRepo> _appRepo = new();

  public RestartingGameTest()
  {
    _context = _state.Test();

    _context.Set(_appRepo.Object);
  }

  [Fact]
  public void OnEnter()
  {
    _appRepo.Setup(repo => repo.OnExitGame(PostGameAction.RestartGame));

    _state.Enter();

    _appRepo.VerifyAll();
  }
}
