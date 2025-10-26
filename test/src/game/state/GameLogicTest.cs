namespace GameDemo.Tests;

using Chickensoft.GoDotTest;
using Godot;
using Shouldly;

public class GameLogicTest : TestClass
{
  private GameLogic _logic = default!;

  public GameLogicTest(Node testScene) : base(testScene) { }

  [Setup]
  public void Setup() => _logic = new GameLogic();

  [Test]
  public void Initializes()
  {
    _logic
      .GetInitialState().State
      .ShouldBeAssignableTo<GameLogic.State>();
  }
}
