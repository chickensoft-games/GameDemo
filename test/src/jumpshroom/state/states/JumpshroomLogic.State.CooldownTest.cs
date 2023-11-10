namespace GameDemo.Tests;

using Chickensoft.GoDotTest;
using Godot;
using Shouldly;

public class JumpshroomLogicStateCooldownTest : TestClass {
  private JumpshroomLogic.IFakeContext _context = default!;
  private JumpshroomLogic.State.Cooldown _state = default!;

  public JumpshroomLogicStateCooldownTest(Node testScene) : base(testScene) { }

  [Setup]
  public void Setup() {
    _context = JumpshroomLogic.CreateFakeContext();

    _state = new(_context);
  }

  [Test]
  public void Enters() {
    _state.Enter();

    _context.Outputs.ShouldBe(new object[] {
      new JumpshroomLogic.Output.StartCooldownTimer()
    });
  }

  [Test]
  public void CooldownCompletedGoesToIdle() {
    var next = _state.On(new JumpshroomLogic.Input.CooldownCompleted());

    next.ShouldBeOfType<JumpshroomLogic.State.Idle>();
  }
}
