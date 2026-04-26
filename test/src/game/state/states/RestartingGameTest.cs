namespace GameDemo.Tests;

using Chickensoft.GoDotTest;
using Chickensoft.LogicBlocks;
using Godot;
using Moq;

public class RestartingGameTest : TestClass
{
  private StateTester _context = default!;
  private GameLogic.BaseState.RestartingGame _state = default!;
  private Mock<IAppRepo> _appRepo = default!;

  public RestartingGameTest(Node testScene) : base(testScene) { }

  [Setup]
  public void Setup()
  {
    _state = new GameLogic.BaseState.RestartingGame();
    _context = _state.Test();

    _appRepo = new Mock<IAppRepo>();
    _context.Set(_appRepo.Object);
  }

  [Test]
  public void OnEnter()
  {
    _appRepo.Setup(repo => repo.OnExitGame(PostGameAction.RestartGame));

    _state.Enter();

    _appRepo.VerifyAll();
  }
}
