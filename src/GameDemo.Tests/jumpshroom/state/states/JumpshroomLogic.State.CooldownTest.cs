namespace GameDemo.Tests;

using Shouldly;

public class JumpshroomLogicStateCooldownTest
{
  private readonly StateTester _context;
  private readonly JumpshroomLogicState.Cooldown _state = new();

  public JumpshroomLogicStateCooldownTest()
  {
    _context = _state.Test();
  }

  [Fact]
  public void Enters()
  {
    _state.Enter();

    _context.Outputs.ShouldBe([
      new JumpshroomLogicState.Output.StartCooldownTimer()
    ]);
  }

  [Fact]
  public void CooldownCompletedGoesToIdle()
  {
    var next = _state.On(new JumpshroomLogicState.Input.CooldownCompleted());

    next.IsAssignableTo(typeof(JumpshroomLogicState.Idle)).ShouldBeTrue();
  }
}
