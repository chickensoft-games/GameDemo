namespace GameDemo.Tests;

using Chickensoft.GoDotTest;
using Godot;
using Moq;
using Shouldly;

public class PlayerLogicStateAliveGroundedMovingTest : TestClass {
  private PlayerLogic.IFakeContext _context = default!;
  private Mock<IAppRepo> _appRepo = default!;
  private PlayerLogic.State.Moving _state = default!;

  public PlayerLogicStateAliveGroundedMovingTest(Node testScene) :
    base(testScene) { }

  [Setup]
  public void Setup() {
    _context = PlayerLogic.CreateFakeContext();
    _appRepo = new();

    _context.Set(_appRepo.Object);
    _state = new(_context);
  }

  [Test]
  public void Enters() {
    var parent =
      new PlayerLogic.State.Grounded(new Mock<PlayerLogic.IContext>().Object);
    _state.Enter(parent);

    _context.Outputs.ShouldBe(new object[] {
      new PlayerLogic.Output.Animations.Move()
    });
  }

  [Test]
  public void OnStoppedMovingHorizontallyIdles() {
    var next = _state.On(new PlayerLogic.Input.StoppedMovingHorizontally());

    next.ShouldBeAssignableTo<PlayerLogic.State.Idle>();
  }
}
