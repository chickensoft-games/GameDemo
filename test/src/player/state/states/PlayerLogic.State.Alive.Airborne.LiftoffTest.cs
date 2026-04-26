namespace GameDemo.Tests;

using Chickensoft.GoDotTest;
using Chickensoft.LogicBlocks;
using Godot;
using Shouldly;

public class PlayerLogicStateAliveAirborneLiftoffTest : TestClass
{
  private StateTester _context = default!;
  private PlayerLogic.BaseState.Liftoff _state = default!;

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
    _state.Enter(new PlayerLogic.BaseState.Idle());

    _context.Outputs.ShouldBe([
      new PlayerLogic.Output.Animations.Jump()
    ]);
  }
}
