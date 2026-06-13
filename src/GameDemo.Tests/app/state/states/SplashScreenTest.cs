namespace GameDemo.Tests;

using Godot;
using Moq;
using Shouldly;

public class SplashScreenTest(GodotHeadlessFixture godot)
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

  [Fact]
  public void OnEnter()
  {
    _state.Enter();
    _context.Outputs.ShouldBe([new AppLogicState.Output.ShowSplashScreen()]);
  }

  [Fact]
  public void RespondsToFadeOutFinished()
  {
    var next = _state.On(new AppLogicState.Input.FadeOutFinished());

    next.IsAssignableTo(typeof(AppLogicState.MainMenu)).ShouldBeTrue();
  }

  [Fact]
  public void SkipsSplashScreen()
  {
    _state.OnSplashScreenSkipped();

    _context.Outputs.ShouldBe([new AppLogicState.Output.HideSplashScreen()]);
  }
}
