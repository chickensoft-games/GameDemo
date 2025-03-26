namespace GameDemo.Tests;

using Chickensoft.GoDotTest;
using Chickensoft.LogicBlocks;
using Godot;
using Moq;
using Shouldly;

public class SplashScreenTest : TestClass {
  private IFakeContext _context = default!;
  private Mock<IAppRepo> _appRepo = default!;
  private AppLogic.State.SplashScreen _state = default!;
  private AppLogic.Data _data = default!;

  public SplashScreenTest(Node testScene) : base(testScene) { }

  [Setup]
  public void Setup() {
    _state = new();
    _appRepo = new();
    _data = new();
    _context = _state.CreateFakeContext();
    _context.Set(_appRepo.Object);
    _context.Set(_data);
  }

  [Test]
  public void OnEnter() {
    _state.Enter();
    _context.Outputs.ShouldBe([new AppLogic.Output.ShowSplashScreen()]);
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

    next.State.ShouldBeOfType<AppLogic.State.MainMenu>();
  }

  [Test]
  public void SkipsSplashScreen() {
    _state.OnSplashScreenSkipped();

    _context.Outputs.ShouldBe([new AppLogic.Output.HideSplashScreen()]);
  }
}
