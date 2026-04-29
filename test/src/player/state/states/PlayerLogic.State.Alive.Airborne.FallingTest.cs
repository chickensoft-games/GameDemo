namespace GameDemo.Tests;

using Chickensoft.GoDotTest;
using Chickensoft.LogicBlocks;
using Godot;
using Shouldly;

public class PlayerLogicStateAliveAirborneFallingTest : TestClass
{
  private StateTester _context = default!;
  private PlayerLogic.BaseState.Falling _state = default!;

  public PlayerLogicStateAliveAirborneFallingTest(Node testScene) :
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
      new PlayerLogic.Output.Animations.Fall()
    ]);
  }
}
