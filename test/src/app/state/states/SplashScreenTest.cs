namespace GameDemo.Tests;

using Chickensoft.GoDotTest;
using Godot;
using Moq;
using Shouldly;

public class SplashScreenTest : TestClass {
  private AppLogic.IFakeContext _context = default!;
  private Mock<IAppRepo> _appRepo = default!;
  private AppLogic.State.SplashScreen _state = default!;

  public SplashScreenTest(Node testScene) : base(testScene) { }

  [Setup]
  public void Setup() {
    _context = AppLogic.CreateFakeContext();
    _appRepo = new();

    _context.Set(_appRepo.Object);
    _state = new(_context);
  }

  [Test]
  public void EntersAndExits() {
    // We pass an instance of our parent class to the enter method as the
    // previous state so that we don't have to run its entrance callbacks
    // (that class has its own unit tests). Telling LogicBlocks that it was the
    // previous state means that we've already "entered" that part of the state
    // hierarchy.
    var parent = new AppLogic.State(_context);

    _state.Enter(parent);

    _appRepo.VerifyAdd(
      repo => repo.SplashScreenSkipped += _state.OnSplashScreenSkipped
    );

    // Likewise, we pass the parent class to the exit method as the next state
    // to prevent us from "leaving" the current state, which prevents the parent
    // exit callbacks from running.
    _state.Exit(parent);

    _appRepo.VerifyRemove(
      repo => repo.SplashScreenSkipped -= _state.OnSplashScreenSkipped
    );
  }

  [Test]
  public void RespondsToFadeOutFinished() {
    var next = _state.On(new AppLogic.Input.FadeOutFinished());

    next.ShouldBeOfType<AppLogic.State.MainMenu>();
  }

  [Test]
  public void SkipsSplashScreen() {
    _state.OnSplashScreenSkipped();

    _context.Outputs.ShouldBe(
      new object[] { new AppLogic.Output.HideSplashScreen() }
    );
  }
}
