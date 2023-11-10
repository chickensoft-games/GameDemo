namespace GameDemo.Tests;

using Chickensoft.GoDotTest;
using Godot;
using Shouldly;

public class PlayerLogicStateDeadTest : TestClass {
  public PlayerLogicStateDeadTest(Node testScene) : base(testScene) { }

  [Test]
  public void Initializes() {
    var context = PlayerLogic.CreateFakeContext();
    var state = new PlayerLogic.State.Dead(context);

    state.ShouldNotBeNull();
  }
}
