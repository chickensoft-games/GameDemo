namespace GameDemo.Tests;

using Chickensoft.GoDotTest;
using Chickensoft.LogicBlocks;
using Godot;
using Moq;
using Shouldly;

public class MainMenuTest : TestClass
{
  private IFakeContext _context = default!;
  private Mock<IAppRepo> _appRepo = default!;
  private AppLogic.State.MainMenu _state = default!;
  private AppLogic.Data _data = default!;


  public MainMenuTest(Node testScene) : base(testScene) { }

  [Setup]
  public void Setup()
  {
    _state = new();
    _appRepo = new();
    _data = new();
    _context = _state.CreateFakeContext();
    _context.Set(_appRepo.Object);
    _context.Set(_data);
  }

  [Test]
  public void Enters()
  {
    _state.Enter();

    _context.Outputs.ShouldBe([
      new AppLogic.Output.SetupGameScene(),
      new AppLogic.Output.ShowMainMenu()
    ]);
  }

  [Test]
  public void StartsGame()
  {
    var next = _state.On(new AppLogic.Input.NewGame());

    next.State.ShouldBeOfType<AppLogic.State.LeavingMenu>();
  }

  [Test]
  public void LeavesMenuOnLoadGame()
  {
    var next = _state.On(new AppLogic.Input.LoadGame());

    next.State.ShouldBeOfType<AppLogic.State.LeavingMenu>();

    _data.ShouldLoadExistingGame.ShouldBeTrue();
  }
}
