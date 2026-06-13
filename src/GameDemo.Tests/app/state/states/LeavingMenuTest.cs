namespace GameDemo.Tests;

using Godot;
using Moq;
using Shouldly;

public class LeavingMenuTest(GodotHeadlessFixture godot)
{
  private StateTester _context = default!;
  private Mock<IAppRepo> _appRepo = default!;
  private AppLogicState.LeavingMenu _state = default!;
  private AppLogic.Data _data = default!;

  public LeavingMenuTest(Node testScene) : base(testScene) { }

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
