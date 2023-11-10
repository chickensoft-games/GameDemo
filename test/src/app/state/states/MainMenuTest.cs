namespace GameDemo.Tests;

using Chickensoft.GoDotTest;
using Godot;
using Moq;
using Shouldly;

public class MainMenuTest : TestClass {
  private AppLogic.IFakeContext _context = default!;
  private Mock<IAppRepo> _appRepo = default!;
  private AppLogic.State.MainMenu _state = default!;

  public MainMenuTest(Node testScene) : base(testScene) { }

  [Setup]
  public void Setup() {
    _context = AppLogic.CreateFakeContext();
    _appRepo = new();

    _context.Set(_appRepo.Object);
    _state = new(_context);
  }

  [Test]
  public void Enters() {
    var parent = new AppLogic.State(_context);

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
