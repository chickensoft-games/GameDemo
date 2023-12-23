namespace GameDemo.Tests;

using Chickensoft.GoDotCollections;
using Chickensoft.GoDotTest;
using Chickensoft.LogicBlocks;
using Godot;
using Moq;
using Shouldly;

public class SplashScreenTest : TestClass {
  private IFakeContext _context = default!;
  private Mock<IAppRepo> _appRepo = default!;
  private AppLogic.State.SplashScreen _state = default!;
  private AutoProp<bool> _isMouseCaptured = default!;

  public SplashScreenTest(Node testScene) : base(testScene) { }

  [Setup]
  public void Setup() {
    _appRepo = new();
    _isMouseCaptured = new(false);
    _appRepo.Setup(repo => repo.IsMouseCaptured)
      .Returns(_isMouseCaptured);

    _state = new();
    _context = _state.CreateFakeContext();
    _context.Set(_appRepo.Object);
  }

  [Test]
  public void Subscribes() {
    _state.Attach(_context);

    _appRepo.VerifyAdd(
      repo => repo.SplashScreenSkipped += _state.OnSplashScreenSkipped
    );

    // Likewise, we pass the parent class to the exit method as the next state
    // to prevent us from "leaving" the current state, which prevents the parent
    // exit callbacks from running.
    _state.Detach();

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
