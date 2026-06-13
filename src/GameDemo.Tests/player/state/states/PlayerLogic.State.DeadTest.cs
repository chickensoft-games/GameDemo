namespace GameDemo.Tests;

using Shouldly;

public class PlayerLogicStateDeadTest
{
  [Fact]
  public void Initializes()
  {
    var state = new PlayerLogicState.Dead();

    state.ShouldNotBeNull();
  }
}
