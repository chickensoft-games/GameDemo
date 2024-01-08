namespace GameDemo.Tests;

using System.Linq;
using Chickensoft.GoDotTest;
using Chickensoft.LogicBlocks;
using Godot;
using Moq;
using Shouldly;

public class InGameTest : TestClass {
  private IFakeContext _context = default!;
  private AppLogic.State.InGame _state = default!;
  private Mock<IAppRepo> _appRepo = default!;

  public InGameTest(Node testScene) : base(testScene) { }

  [Setup]
  public void Setup() {
    _state = new AppLogic.State.InGame();
    _context = _state.CreateFakeContext();

    _appRepo = new Mock<IAppRepo>();
    _context.Set(_appRepo.Object);
  }

  [Test]
  public void OnEnter() {
    _appRepo.Setup(repo => repo.OnEnterGame());

    _state.Enter();

    _appRepo.VerifyAll();
    _context.Outputs.First().ShouldBeOfType<AppLogic.Output.ShowGame>();
  }

  [Test]
  public void Subscribes() {
    _state.Attach(_context);

    _appRepo.VerifyAdd(repo => repo.GameExited += _state.OnGameExited);

    _state.Detach();

    _appRepo.VerifyRemove(repo => repo.GameExited -= _state.OnGameExited);
  }

  [Test]
  public void OnRestartGameRequested() {
    _state.OnRestartGameRequested();

    var input = _context.Inputs.Single()
      .ShouldBeOfType<AppLogic.Input.EndGame>();

    input.PostGameAction.ShouldBe(PostGameAction.RestartGame);
  }

  [Test]
  public void OnGameExited() {
    _state.OnGameExited(PostGameAction.RestartGame);

    var input = _context.Inputs.Single()
      .ShouldBeOfType<AppLogic.Input.EndGame>();

    input.PostGameAction.ShouldBe(PostGameAction.RestartGame);
  }

  [Test]
  public void OnEndGame() {
    var result =
      _state.On(new AppLogic.Input.EndGame(PostGameAction.RestartGame));
    result.ShouldBeOfType<AppLogic.State.LeavingGame>();
  }
}
