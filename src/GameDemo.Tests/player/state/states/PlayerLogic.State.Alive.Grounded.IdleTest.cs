namespace GameDemo.Tests;

using Shouldly;

public class PlayerLogicStateAliveGroundedIdleTest
{
  private readonly StateTester _context;
  private readonly PlayerLogicState.Idle _state = new();

  public PlayerLogicStateAliveGroundedIdleTest()
  {
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
