namespace GameDemo.Tests;

using Chickensoft.GoDotTest;
using Chickensoft.LogicBlocks;
using Godot;
using Moq;
using Shouldly;

public class LeavingMenuTest : TestClass {
  private IFakeContext _context = default!;
  private Mock<IAppRepo> _appRepo = default!;
  private AppLogic.State.LeavingMenu _state = default!;

  public LeavingMenuTest(Node testScene) : base(testScene) { }

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
        new AppLogic.Output.FadeOut(),
      }
    );
  }

  [Test]
  public void StartsGame() {
    var next = _state.On(new AppLogic.Input.FadeOutFinished());

    next.ShouldBeOfType<AppLogic.State.PlayingGame>();
  }
}
