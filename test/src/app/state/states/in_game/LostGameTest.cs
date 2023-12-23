namespace GameDemo.Tests;

using Chickensoft.GoDotTest;
using Chickensoft.LogicBlocks;
using Godot;
using Moq;
using Shouldly;

public class LostGameTest : TestClass {
  private IFakeContext _context = default!;
  private Mock<IAppRepo> _appRepo = default!;
  private AppLogic.State.LostGame _state = default!;

  public LostGameTest(Node testScene) : base(testScene) { }

  [Setup]
  public void Setup() {
    _appRepo = new();

    _state = new();
    _context = _state.CreateFakeContext();
    _context.Set(_appRepo.Object);
  }

  [Test]
  public void Enters() {
    var parent = new AppLogic.State.InGame();

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
