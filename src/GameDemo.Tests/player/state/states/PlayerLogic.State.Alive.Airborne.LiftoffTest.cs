namespace GameDemo.Tests;

using Shouldly;

public class PlayerLogicStateAliveAirborneLiftoffTest
{
  private readonly StateTester _context;
  private readonly PlayerLogicState.Liftoff _state = new();

  public PlayerLogicStateAliveAirborneLiftoffTest()
  {
    _context = _state.Test();
  }

  [Fact]
  public void Enters()
  {
    _state.Enter();

    _context.Outputs.ShouldBe([
      new PlayerLogicState.Output.Animations.Jump()
    ]);
  }
}
