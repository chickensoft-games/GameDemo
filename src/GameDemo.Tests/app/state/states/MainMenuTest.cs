namespace GameDemo.Tests;

using Chickensoft.GoDotTest;
using Chickensoft.LogicBlocks;
using Godot;
using Moq;
using Shouldly;

public class MainMenuTest : TestClass
{
  private StateTester _context = default!;
  private Mock<IAppRepo> _appRepo = default!;
  private AppLogicState.MainMenu _state = default!;
  private AppLogic.Data _data = default!;


  public MainMenuTest(Node testScene) : base(testScene) { }

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

    _context.Outputs.ShouldBe([
      new AppLogicState.Output.SetupGameScene(),
      new AppLogicState.Output.ShowMainMenu()
    ]);
  }

  [Test]
  public void StartsGame()
  {
    var next = _state.On(new AppLogicState.Input.NewGame());

    next.IsAssignableTo(typeof(AppLogicState.LeavingMenu)).ShouldBeTrue();
  }

  [Test]
  public void LeavesMenuOnLoadGame()
  {
    var next = _state.On(new AppLogicState.Input.LoadGame());

    next.IsAssignableTo(typeof(AppLogicState.LeavingMenu)).ShouldBeTrue();

    _data.ShouldLoadExistingGame.ShouldBeTrue();
  }
}
