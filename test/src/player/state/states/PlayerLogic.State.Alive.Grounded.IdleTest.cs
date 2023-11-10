namespace GameDemo.Tests;

using Chickensoft.GoDotTest;
using Godot;
using Moq;
using Shouldly;

public class PlayerLogicStateAliveGroundedIdleTest : TestClass {
  private PlayerLogic.IFakeContext _context = default!;
  private PlayerLogic.State.Idle _state = default!;

  public PlayerLogicStateAliveGroundedIdleTest(Node testScene) :
    base(testScene) { }

  [Setup]
  public void Setup() {
    _context = PlayerLogic.CreateFakeContext();
    _state = new(_context);
  }

  [Test]
  public void Enters() {
    var parent =
      new PlayerLogic.State.Grounded(new Mock<PlayerLogic.IContext>().Object);
    _state.Enter(parent);

    _context.Outputs.ShouldBe(new object[] {
      new PlayerLogic.Output.Animations.Idle()
    });
  }

  [Test]
  public void OnStartedMovingHorizontallyGoesToMoving() {
    var next = _state.On(new PlayerLogic.Input.StartedMovingHorizontally());

    next.ShouldBeAssignableTo<PlayerLogic.State.Moving>();
  }
}
