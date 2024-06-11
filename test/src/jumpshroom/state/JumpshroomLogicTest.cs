namespace GameDemo.Tests;

using Chickensoft.GoDotTest;
using Godot;
using Shouldly;

public class JumpshroomLogicTest : TestClass {
  private JumpshroomLogic _logic = default!;

  public JumpshroomLogicTest(Node testScene) : base(testScene) { }

  [Setup]
  public void Setup() {
    _logic = new JumpshroomLogic();
  }

  [Test]
  public void Initializes() {
    _logic
      .GetInitialState().State
      .ShouldBeAssignableTo<JumpshroomLogic.State>();
  }
}
