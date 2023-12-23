namespace GameDemo.Tests;

using Chickensoft.GoDotTest;
using Chickensoft.LogicBlocks;
using Godot;
using Moq;
using Shouldly;

public class MainMenuTest : TestClass {
  private IFakeContext _context = default!;
  private Mock<IAppRepo> _appRepo = default!;
  private AppLogic.State.MainMenu _state = default!;

  public MainMenuTest(Node testScene) : base(testScene) { }

  [Setup]
  public void Setup() {
    _appRepo = new();

    _state = new();
    _context = _state.CreateFakeContext();
    _context.Set(_appRepo.Object);
  }

  [Test]
  public void Enters() {
    var parent = new AppLogic.State();

    _state.Enter(parent);

    _context.Outputs.ShouldBe(
      new object[] {
        new AppLogic.Output.LoadGame(),
        new AppLogic.Output.ShowMainMenu()
      }
    );
  }

  [Test]
  public void StartsGame() {
    var next = _state.On(new AppLogic.Input.StartGame());

    next.ShouldBeOfType<AppLogic.State.LeavingMenu>();
  }
}
