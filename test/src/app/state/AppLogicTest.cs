namespace GameDemo.Tests;

using Chickensoft.GoDotTest;
using Godot;
using Shouldly;

public class AppLogicTest : TestClass
{
  private AppLogic _logic = default!;

  public AppLogicTest(Node testScene) : base(testScene) { }

  [Setup]
  public void Setup() => _logic = new AppLogic();

  [Test]
  public void Initializes()
  {
    _logic
      .GetInitialState().State
      .ShouldBeAssignableTo<AppLogic.State>();
  }
}
