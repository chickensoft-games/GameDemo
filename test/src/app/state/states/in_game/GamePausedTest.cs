namespace GameDemo.Tests;

using Chickensoft.GoDotTest;
using Godot;
using Moq;
using Shouldly;

public class GamePausedTest : TestClass {
  private AppLogic.IFakeContext _context = default!;
  private Mock<IAppRepo> _appRepo = default!;
  private AppLogic.State.GamePaused _state = default!;

  public GamePausedTest(Node testScene) : base(testScene) { }

  [Setup]
  public void Setup() {
    _context = AppLogic.CreateFakeContext();
    _appRepo = new();

    _context.Set(_appRepo.Object);
    _state = new(_context);
  }

  [Test]
  public void EntersAndExits() {
    var parent = new AppLogic.State.InGame(_context);

    _appRepo.Setup((repo) => repo.Pause());

    _state.Enter(parent);

    _context.Outputs.ShouldBe(
      new object[] { new AppLogic.Output.ShowPauseMenu() }
    );
    _appRepo.VerifyAll();

    _context.Reset();

    _state.Exit(parent);

    _context.Outputs.ShouldBe(
      new object[] { new AppLogic.Output.HidePauseMenu() }
    );
  }

  [Test]
  public void ResumesGame() {
    var next = _state.On(new AppLogic.Input.PauseButtonPressed());

    next.ShouldBeOfType<AppLogic.State.ResumingGame>();
  }
}
