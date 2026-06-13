namespace GameDemo.Tests;

using Moq;
using Shouldly;

public class SplashScreenTest
{
  private readonly StateTester _context;
  private readonly Mock<IAppRepo> _appRepo = new();
  private readonly AppLogicState.SplashScreen _state = new();
  private readonly AppLogic.Data _data = new();

  public SplashScreenTest()
  {
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
