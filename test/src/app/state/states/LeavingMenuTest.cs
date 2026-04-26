namespace GameDemo.Tests;

using Chickensoft.GoDotTest;
using Chickensoft.LogicBlocks;
using Godot;
using Moq;
using Shouldly;

public class LeavingMenuTest : TestClass
{
  private StateTester _context = default!;
  private Mock<IAppRepo> _appRepo = default!;
  private AppLogic.BaseState.LeavingMenu _state = default!;
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

  [Test]
  public void Enters()
  {
    _state.Enter();

    _context.Outputs.ShouldBe(
      [new AppLogic.Output.FadeToBlack()]
    );
  }

  [Test]
  public void StartsGameOnFadeOutFinished()
  {
    var next = _state.On(new AppLogic.Input.FadeOutFinished());

    next.IsAssignableTo(typeof(AppLogic.BaseState.InGame)).ShouldBeTrue();
  }

  [Test]
  public void LoadsSaveFileOnFadeOutFinished()
  {
    _data.ShouldLoadExistingGame = true;

    var next = _state.On(new AppLogic.Input.FadeOutFinished());

    next.IsAssignableTo(typeof(AppLogic.BaseState.LoadingSaveFile)).ShouldBeTrue();
  }
}
