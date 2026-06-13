namespace GameDemo.Tests;

using Godot;
using Moq;

public class RestartingGameTest(GodotHeadlessFixture godot)
{
  private StateTester _context = default!;
  private GameLogicState.RestartingGame _state = default!;
  private Mock<IAppRepo> _appRepo = default!;

  public RestartingGameTest(Node testScene) : base(testScene) { }

  [Setup]
  public void Setup()
  {
    _state = new GameLogicState.RestartingGame();
    _context = _state.Test();

    _appRepo = new Mock<IAppRepo>();
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
