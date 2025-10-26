namespace GameDemo.Tests;

using Chickensoft.GoDotTest;
using Chickensoft.LogicBlocks;
using Godot;
using Shouldly;

public class PlayerLogicStateAliveAirborneLiftoffTest : TestClass
{
  private IFakeContext _context = default!;
  private PlayerLogic.State.Liftoff _state = default!;

  public PlayerLogicStateAliveAirborneLiftoffTest(Node testScene) :
    base(testScene)
  { }

  [Setup]
  public void Setup()
  {
    _state = new();
    _context = _state.CreateFakeContext();
  }

  [Test]
  public void Enters()
  {
    _state.Enter<PlayerLogic.State.Airborne>();

    _context.Outputs.ShouldBe([
      new PlayerLogic.Output.Animations.Jump()
    ]);
  }
}
