namespace GameDemo.Tests;

using Godot;
using Shouldly;

public class PlayerLogicStateAliveAirborneLiftoffTest(GodotHeadlessFixture godot)
{
  private StateTester _context = default!;
  private PlayerLogicState.Liftoff _state = default!;

  public PlayerLogicStateAliveAirborneLiftoffTest(Node testScene) :
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
      new PlayerLogicState.Output.Animations.Jump()
    ]);
  }
}
