namespace GameDemo.Tests;

using Shouldly;

public class PlayerLogicStateAliveAirborneFallingTest
{
  private readonly StateTester _context;
  private readonly PlayerLogicState.Falling _state = new();

  public PlayerLogicStateAliveAirborneFallingTest()
  {
    _context = _state.Test();
  }

  [Fact]
  public void Enters()
  {
    _state.Enter();

    _context.Outputs.ShouldBe([
      new PlayerLogicState.Output.Animations.Fall()
    ]);
  }
}
