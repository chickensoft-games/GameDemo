namespace GameDemo.Tests;

using Chickensoft.GoDotTest;
using Chickensoft.LogicBlocks;
using Godot;
using Moq;

public class QuitTest : TestClass {
  private IFakeContext _context = default!;
  private GameLogic.State.Quit _state = default!;
  private Mock<IAppRepo> _appRepo = default!;


  public QuitTest(Node testScene) : base(testScene) { }

  [Setup]
  public void Setup() {
    _state = new GameLogic.State.Quit();
    _context = _state.CreateFakeContext();

    _appRepo = new Mock<IAppRepo>();
    _context.Set(_appRepo.Object);
  }

  [Test]
  public void OnEnter() {
    _appRepo.Setup(repo => repo.OnExitGame(PostGameAction.GoToMainMenu));

    _state.Enter();

    _appRepo.VerifyAll();
  }
}
