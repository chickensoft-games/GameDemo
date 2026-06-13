namespace GameDemo.Tests;

using Moq;

public class QuitTest
{
  private readonly StateTester _context;
  private readonly GameLogicState.Quit _state = new();
  private readonly Mock<IAppRepo> _appRepo = new();

  public QuitTest()
  {
    _context = _state.Test();

    _context.Set(_appRepo.Object);
  }

  [Fact]
  public void OnEnter()
  {
    _appRepo.Setup(repo => repo.OnExitGame(PostGameAction.GoToMainMenu));

    _state.Enter();

    _appRepo.VerifyAll();
  }
}
