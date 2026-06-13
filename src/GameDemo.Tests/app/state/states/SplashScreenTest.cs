namespace GameDemo.Tests;

using Godot;
using Moq;
using Shouldly;

public class SplashScreenTest : TestClass
{
  private StateTester _context = default!;
  private Mock<IAppRepo> _appRepo = default!;
  private AppLogicState.SplashScreen _state = default!;
  private AppLogic.Data _data = default!;

  public SplashScreenTest(Node testScene) : base(testScene) { }

  [Setup]
  public void Setup()
  {
    _state = new();
    _appRepo = new();
    _data = new();
    _context = _state.Test();
    _context.Set(_appRepo.Object);
    _context.Set(_data);
  }

  [Test]
  public void OnEnter()
  {
    _state.Enter();
    _context.Outputs.ShouldBe([new AppLogicState.Output.ShowSplashScreen()]);
  }

  [Test]
  public void RespondsToFadeOutFinished()
  {
    var next = _state.On(new AppLogicState.Input.FadeOutFinished());

    next.IsAssignableTo(typeof(AppLogicState.MainMenu)).ShouldBeTrue();
  }

  [Test]
  public void SkipsSplashScreen()
  {
    _state.OnSplashScreenSkipped();

    _context.Outputs.ShouldBe([new AppLogicState.Output.HideSplashScreen()]);
  }
}
