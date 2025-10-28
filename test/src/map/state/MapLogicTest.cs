namespace GameDemo.Tests;

using Chickensoft.GoDotTest;
using Godot;
using Shouldly;

public class MapLogicTest : TestClass
{
  private MapLogic _logic = default!;

  public MapLogicTest(Node testScene) : base(testScene) { }

  [Setup]
  public void Setup() => _logic = new MapLogic();

  [Test]
  public void Initializes()
  {
    _logic
      .GetInitialState().State
      .ShouldBeAssignableTo<MapLogic.State>();
  }
}
