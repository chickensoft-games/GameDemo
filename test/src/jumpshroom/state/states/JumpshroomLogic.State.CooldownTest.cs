namespace GameDemo.Tests;

using Chickensoft.GoDotTest;
using Chickensoft.LogicBlocks;
using Godot;
using Shouldly;

public class JumpshroomLogicStateCooldownTest : TestClass {
  private IFakeContext _context = default!;
  private JumpshroomLogic.State.Cooldown _state = default!;

  public JumpshroomLogicStateCooldownTest(Node testScene) : base(testScene) { }

  [Setup]
  public void Setup() {
    _state = new();
    _context = _state.CreateFakeContext();
  }

  [Test]
  public void Enters() {
    _state.Enter();

    _context.Outputs.ShouldBe([
      new JumpshroomLogic.Output.StartCooldownTimer()
    ]);
  }

  [Test]
  public void CooldownCompletedGoesToIdle() {
    var next = _state.On(new JumpshroomLogic.Input.CooldownCompleted());

    next.State.ShouldBeOfType<JumpshroomLogic.State.Idle>();
  }
}
