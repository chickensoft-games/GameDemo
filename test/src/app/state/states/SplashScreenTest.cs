namespace GameDemo.Tests;

using Chickensoft.GoDotTest;
using Godot;
using Shouldly;

public class SplashScreenTest : TestClass
{
  private StateTester _context = default!;
  private IAppRepo _appRepo = default!;
  private AppLogic.BaseState.SplashScreen _state = default!;
  private AppLogic.Data _data = default!;

  public SplashScreenTest(Node testScene) : base(testScene) { }

  [Setup]
  public void Setup()
  {
    _state = new();
    _appRepo = new AppRepo();
    _data = new();
    _context = _state.Test();
    _context.Set(_appRepo);
    _context.Set(_data);
    AppLogic.SetupSubscriptions(_appRepo, () => _state);
  }

  [Cleanup]
  public void Cleanup() => _appRepo.Dispose();

  [Test]
  public void OnEnter()
  {
    _state.Enter();
    _context.Outputs.ShouldBe([new AppLogic.Output.ShowSplashScreen()]);
  }

  [Test]
  public void RespondsToFadeOutFinished()
  {
    var next = _state.On(new AppLogic.Input.FadeOutFinished());

    next.IsAssignableTo(typeof(AppLogic.BaseState.MainMenu)).ShouldBeTrue();
  }

  [Test]
  public void SkipsSplashScreen()
  {
    _appRepo.SkipSplashScreen();

    _context.Outputs.ShouldBe([new AppLogic.Output.HideSplashScreen()]);
  }
}
