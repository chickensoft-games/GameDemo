namespace GameDemo.Tests;

using Moq;
using Shouldly;

public class LeavingMenuTest
{
  private readonly StateTester _context;
  private readonly Mock<IAppRepo> _appRepo = new();
  private readonly AppLogicState.LeavingMenu _state = new();
  private readonly AppLogic.Data _data = new();

  public LeavingMenuTest()
  {
    _context = _state.Test();
    _context.Set(_appRepo.Object);
    _context.Set(_data);
  }

  [Fact]
  public void Enters()
  {
    _state.Enter();

    _context.Outputs.ShouldBe(
      [new AppLogicState.Output.FadeToBlack()]
    );
  }

  [Fact]
  public void StartsGameOnFadeOutFinished()
  {
    var next = _state.On(new AppLogicState.Input.FadeOutFinished());

    next.IsAssignableTo(typeof(AppLogicState.InGame)).ShouldBeTrue();
  }

  [Fact]
  public void LoadsSaveFileOnFadeOutFinished()
  {
    _data.ShouldLoadExistingGame = true;

    var next = _state.On(new AppLogicState.Input.FadeOutFinished());

    next.IsAssignableTo(typeof(AppLogicState.LoadingSaveFile)).ShouldBeTrue();
  }
}
