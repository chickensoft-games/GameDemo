namespace GameDemo.Tests;

using Chickensoft.GoDotTest;
using Chickensoft.LogicBlocks;
using Godot;
using Moq;
using Shouldly;

public class ResumingGameTest : TestClass {
  private IFakeContext _context = default!;
  private Mock<IAppRepo> _appRepo = default!;
  private AppLogic.State.ResumingGame _state = default!;

  public ResumingGameTest(Node testScene) : base(testScene) { }

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

    _appRepo.Setup((repo) => repo.Resume());

    _state.Enter(parent);

    _appRepo.VerifyAll();

    _state.Exit(parent);

    _context.Outputs.ShouldBe(
      new object[] { new AppLogic.Output.DisablePauseMenu() }
    );
  }

  [Test]
  public void GoesBackToPlayingGameAfterPauseMenuIsGone() {
    var next = _state.On(new AppLogic.Input.PauseMenuTransitioned());

    next.ShouldBeOfType<AppLogic.State.PlayingGame>();
  }
}
