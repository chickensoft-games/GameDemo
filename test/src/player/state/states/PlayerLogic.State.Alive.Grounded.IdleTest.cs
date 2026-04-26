namespace GameDemo.Tests;

using Chickensoft.GoDotTest;
using Chickensoft.LogicBlocks;
using Godot;
using Shouldly;

public class PlayerLogicStateAliveGroundedIdleTest : TestClass
{
  private StateTester _context = default!;
  private PlayerLogic.BaseState.Idle _state = default!;

  public PlayerLogicStateAliveGroundedIdleTest(Node testScene) :
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
      new PlayerLogic.Output.Animations.Idle()
    ]);
  }

  [Test]
  public void OnStartedMovingHorizontallyGoesToMoving()
  {
    var next = _state.On(new PlayerLogic.Input.StartedMovingHorizontally());

    next.ShouldBeAssignableTo<PlayerLogic.BaseState.Moving>();
  }
}
