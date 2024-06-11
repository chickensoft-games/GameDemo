namespace GameDemo.Tests;

using Chickensoft.GoDotTest;
using Godot;
using Shouldly;

public class InGameUILogicTest : TestClass {
  private InGameUILogic _logic = default!;

  public InGameUILogicTest(Node testScene) : base(testScene) { }

  [Setup]
  public void Setup() {
    _logic =
      new InGameUILogic();
  }

  [Test]
  public void Initializes() {
    _logic
      .GetInitialState().State
      .ShouldBeAssignableTo<InGameUILogic.State>();
  }
}
