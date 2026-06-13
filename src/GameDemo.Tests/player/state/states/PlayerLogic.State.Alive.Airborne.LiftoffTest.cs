namespace GameDemo.Tests;

using Chickensoft.LogicBlocks;
using Godot;
using Shouldly;

public class PlayerLogicStateAliveAirborneLiftoffTest : TestClass
{
  private StateTester _context = default!;
  private PlayerLogicState.Liftoff _state = default!;

  public PlayerLogicStateAliveAirborneLiftoffTest(Node testScene) :
    base(testScene)
  { }

  [Setup]
  public void Setup()
  {
    _state = new();
    _context = _state.Test();
  }

  [Test]
  public void Enters()
  {
    _state.Enter();

    _context.Outputs.ShouldBe([
      new PlayerLogicState.Output.Animations.Jump()
    ]);
  }
}
