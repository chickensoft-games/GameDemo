namespace GameDemo.Tests;

using Godot;
using Shouldly;

public class JumpshroomLogicStateCooldownTest(GodotHeadlessFixture godot)
{
  private StateTester _context = default!;
  private JumpshroomLogicState.Cooldown _state = default!;

  public JumpshroomLogicStateCooldownTest(Node testScene) : base(testScene) { }

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
