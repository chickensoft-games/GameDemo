namespace GameDemo.Tests;

using Godot;
using Shouldly;

public class PlayerLogicStateDeadTest(GodotHeadlessFixture godot)
{
  public PlayerLogicStateDeadTest(Node testScene) : base(testScene) { }

  [Fact]
  public void Initializes()
  {
    var state = new PlayerLogicState.Dead();

    state.ShouldNotBeNull();
  }
}
