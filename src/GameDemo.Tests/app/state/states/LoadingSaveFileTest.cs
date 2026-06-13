namespace GameDemo.Tests;

using Moq;
using Shouldly;

public class LoadingSaveFileTest
{
  private readonly StateTester _context;
  private readonly Mock<IAppRepo> _appRepo = new();
  private readonly AppLogicState.LoadingSaveFile _state = new();
  private readonly AppLogic.Data _data = new();


  public LoadingSaveFileTest()
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
