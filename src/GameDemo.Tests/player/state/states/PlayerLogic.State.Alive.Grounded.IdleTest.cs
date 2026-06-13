namespace GameDemo.Tests;

using Godot;
using Shouldly;

public class PlayerLogicStateAliveGroundedIdleTest(GodotHeadlessFixture godot)
{
  private StateTester _context = default!;
  private PlayerLogicState.Idle _state = default!;

  public PlayerLogicStateAliveGroundedIdleTest(Node testScene) :
    base(testScene)
  { }

  [Setup]
  public void Setup()
  {
    _state = new();
    _context = _state.Test();
  }

  [Fact]
  public void Enters()
  {
    _state.Enter();

    _context.Outputs.ShouldBe([
      new PlayerLogicState.Output.Animations.Idle()
    ]);
  }

  [Fact]
  public void OnStartedMovingHorizontallyGoesToMoving()
  {
    var next = _state.On(new PlayerLogicState.Input.StartedMovingHorizontally());

    next.IsAssignableTo(typeof(PlayerLogicState.Moving));
  }
}
