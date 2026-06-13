namespace GameDemo.Tests;

using Godot;
using Moq;

public class QuitTest(GodotHeadlessFixture godot)
{
  private StateTester _context = default!;
  private GameLogicState.Quit _state = default!;
  private Mock<IAppRepo> _appRepo = default!;

  public QuitTest(Node testScene) : base(testScene) { }

  [Setup]
  public void Setup()
  {
    _state = new GameLogicState.Quit();
    _context = _state.Test();

    _appRepo = new Mock<IAppRepo>();
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
