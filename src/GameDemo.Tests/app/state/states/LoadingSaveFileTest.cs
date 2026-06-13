namespace GameDemo.Tests;

using Godot;
using Moq;
using Shouldly;

public class LoadingSaveFileTest(GodotHeadlessFixture godot)
{
  private StateTester _context = default!;
  private Mock<IAppRepo> _appRepo = default!;
  private AppLogicState.LoadingSaveFile _state = default!;
  private AppLogic.Data _data = default!;

  public LoadingSaveFileTest(Node testScene) : base(testScene) { }

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
      [new AppLogicState.Output.StartLoadingSaveFile()]
    );
  }

  [Fact]
  public void GoesToInGameOnSaveFileLoaded()
  {
    var next = _state.On(new AppLogicState.Input.SaveFileLoaded());

    next.IsAssignableTo(typeof(AppLogicState.InGame)).ShouldBeTrue();
  }
}
