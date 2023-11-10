namespace GameDemo.Tests;

using Chickensoft.GoDotTest;
using Godot;
using Moq;
using Shouldly;

public class PlayerLogicStateAliveAirborneLiftoffTest : TestClass {
  private PlayerLogic.IFakeContext _context = default!;
  private PlayerLogic.State.Liftoff _state = default!;

  public PlayerLogicStateAliveAirborneLiftoffTest(Node testScene) :
    base(testScene) { }

  [Setup]
  public void Setup() {
    _context = PlayerLogic.CreateFakeContext();
    _state = new(_context);
  }

  [Test]
  public void Enters() {
    var parent =
      new PlayerLogic.State.Airborne(new Mock<PlayerLogic.IContext>().Object);

    _state.Enter(parent);

    _context.Outputs.ShouldBe(new object[] {
      new PlayerLogic.Output.Animations.Jump()
    });
  }
}
