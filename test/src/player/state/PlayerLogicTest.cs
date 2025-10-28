namespace GameDemo.Tests;

using Chickensoft.GoDotTest;
using Godot;
using Shouldly;

public class PlayerLogicTest : TestClass
{
  private PlayerLogic _logic = default!;

  public PlayerLogicTest(Node testScene) : base(testScene) { }

  [Setup]
  public void Setup() => _logic = new PlayerLogic();

  [Test]
  public void Initializes()
  {
    _logic
      .GetInitialState().State
      .ShouldBeAssignableTo<PlayerLogic.State>();
  }
}
