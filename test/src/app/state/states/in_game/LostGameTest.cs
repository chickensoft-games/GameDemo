namespace GameDemo.Tests;

using Chickensoft.GoDotTest;
using Godot;
using Moq;
using Shouldly;

public class LostGameTest : TestClass {
  private AppLogic.IFakeContext _context = default!;
  private Mock<IAppRepo> _appRepo = default!;
  private AppLogic.State.LostGame _state = default!;

  public LostGameTest(Node testScene) : base(testScene) { }

  [Setup]
  public void Setup() {
    _context = AppLogic.CreateFakeContext();
    _appRepo = new();

    _context.Set(_appRepo.Object);
    _state = new(_context);
  }

  [Test]
  public void Enters() {
    var parent = new AppLogic.State.InGame(_context);

    _state.Enter(parent);

    _context.Outputs.ShouldBe(
      new object[] { new AppLogic.Output.ShowPlayerDied() }
    );
  }

  [Test]
  public void RestartsGame() {
    var next = _state.On(new AppLogic.Input.StartGame());

    next.ShouldBeOfType<AppLogic.State.RestartingGame>();
  }
}
