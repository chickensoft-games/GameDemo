namespace GameDemo.Tests;

using Moq;
using Shouldly;

public class MainMenuTest
{
  private readonly StateTester _context;
  private readonly Mock<IAppRepo> _appRepo = new();
  private readonly AppLogicState.MainMenu _state = new();
  private readonly AppLogic.Data _data = new();

  public MainMenuTest()
  {
    _context = _state.Test();
    _context.Set(_appRepo.Object);
    _context.Set(_data);
  }

  [Fact]
  public void Enters()
  {
    _state.Enter();

    _context.Outputs.ShouldBe([
      new AppLogicState.Output.SetupGameScene(),
      new AppLogicState.Output.ShowMainMenu()
    ]);
  }

  [Fact]
  public void StartsGame()
  {
    var next = _state.On(new AppLogicState.Input.NewGame());

    next.IsAssignableTo(typeof(AppLogicState.LeavingMenu)).ShouldBeTrue();
  }

  [Fact]
  public void LeavesMenuOnLoadGame()
  {
    var next = _state.On(new AppLogicState.Input.LoadGame());

    next.IsAssignableTo(typeof(AppLogicState.LeavingMenu)).ShouldBeTrue();

    _data.ShouldLoadExistingGame.ShouldBeTrue();
  }
}
