namespace GameDemo.Tests;

using Chickensoft.GoDotTest;
using Godot;
using Moq;
using Shouldly;

public class LeavingMenuTest : TestClass {
  private AppLogic.IFakeContext _context = default!;
  private Mock<IAppRepo> _appRepo = default!;
  private AppLogic.State.LeavingMenu _state = default!;

  public LeavingMenuTest(Node testScene) : base(testScene) { }

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
