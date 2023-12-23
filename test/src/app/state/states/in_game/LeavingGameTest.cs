namespace GameDemo.Tests;

using Chickensoft.GoDotTest;
using Chickensoft.LogicBlocks;
using Godot;
using Moq;
using Shouldly;

public class LeavingGameTest : TestClass {
  private IFakeContext _context = default!;
  private Mock<IAppRepo> _appRepo = default!;
  private AppLogic.State.LeavingGame _state = default!;

  public LeavingGameTest(Node testScene) : base(testScene) { }

  [Setup]
  public void Setup() {
    _appRepo = new();

    _state = new();
    _context = _state.CreateFakeContext();
    _context.Set(_appRepo.Object);
  }

  [Test]
  public void EntersAndExits() {
    var parent = new AppLogic.State.InGame();

    _state.Enter(parent);
    _context.Outputs.ShouldBe(
      new object[] { new AppLogic.Output.FadeOut() }
    );

    _context.Reset();

    _state.Exit(parent);
    _context.Outputs.ShouldBe(
      new object[] { new AppLogic.Output.RemoveExistingGame() }
    );
  }

  [Test]
  public void TellsAppGameEndedOnFadeOut() {
    _appRepo.Setup((repo) => repo.OnGameEnded(GameOverReason.Exited));

    var next = _state.On(new AppLogic.Input.FadeOutFinished());

    _appRepo.VerifyAll();

    next.ShouldBeOfType<AppLogic.State.LeavingGame>();
  }
}
