namespace GameDemo.Tests;

using Chickensoft.GoDotTest;
using Godot;
using Shouldly;

public class ControllerTest : TestClass {
  public ControllerTest(Node testScene) :
  base(testScene) { }

  [Test]
  public void ConfirmPathUpdates() {
    Controller.ConfirmPath.ShouldBe("res://src/shared_ui/assets/Space_Key_Light.png");

    Controller.Current = "PS4 Controller";
    Controller.ConfirmPath.ShouldBe("res://src/shared_ui/assets/PS4_Cross.png");
  }
}

