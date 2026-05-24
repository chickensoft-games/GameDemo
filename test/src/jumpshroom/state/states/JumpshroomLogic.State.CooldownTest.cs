namespace GameDemo.Tests;

using Chickensoft.GoDotTest;
using Chickensoft.LogicBlocks;
using Godot;
using Shouldly;

public class JumpshroomLogicStateCooldownTest : TestClass
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

  [Test]
  public void Enters()
  {
    _state.Enter();

    _context.Outputs.ShouldBe([
      new JumpshroomLogicState.Output.StartCooldownTimer()
    ]);
  }

  [Test]
  public void CooldownCompletedGoesToIdle()
  {
    var next = _state.On(new JumpshroomLogicState.Input.CooldownCompleted());

    next.IsAssignableTo(typeof(JumpshroomLogicState.Idle)).ShouldBeTrue();
  }
}
