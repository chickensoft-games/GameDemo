namespace GameDemo.Tests;

using Godot;
using Shouldly;

public class PlayerLogicStateDeadTest : TestClass
{
  public PlayerLogicStateDeadTest(Node testScene) : base(testScene) { }

  [Test]
  public void Initializes()
  {
    var state = new PlayerLogicState.Dead();

    state.ShouldNotBeNull();
  }
}
