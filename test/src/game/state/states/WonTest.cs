namespace GameDemo.Tests;

using System.Linq;
using Chickensoft.GoDotTest;
using Chickensoft.LogicBlocks;
using Godot;
using Moq;
using Shouldly;

public class WonTest : TestClass {
  private IFakeContext _context = default!;
  private GameLogic.State.Won _state = default!;
  private Mock<IAppRepo> _appRepo = default!;

  public WonTest(Node testScene) : base(testScene) { }

  [Setup]
  public void Setup() {
    _state = new GameLogic.State.Won();
    _context = _state.CreateFakeContext();

    _appRepo = new Mock<IAppRepo>();
    _context.Set(_appRepo.Object);
  }

  [Test]
  public void OnEnter() {
    _state.Enter();
    _context.Outputs.First().ShouldBeOfType<GameLogic.Output.ShowWonScreen>();
  }

  [Test]
  public void OnGoToMainMenu() {
    _appRepo.Setup(repo => repo.OnExitGame(PostGameAction.GoToMainMenu));

    var result = _state.On(new GameLogic.Input.GoToMainMenu());

    _appRepo.VerifyAll();
    result.State.ShouldBe(_state);
  }
}
