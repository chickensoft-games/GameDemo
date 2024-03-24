namespace GameDemo.Tests;

using Chickensoft.GoDotTest;
using Godot;
using Shouldly;

public class InputUtilitiesTest : TestClass {

  private float _deadZoneX = default!;
  private float _deadZoneY = default!;

  public InputUtilitiesTest(Node testScene) :
  base(testScene) { }

  [Setup]
  public void Setup() {
    _deadZoneX = InputMap.ActionGetDeadzone("camera_right");
    _deadZoneY = InputMap.ActionGetDeadzone("camera_up");
  }

  [Test]
  public void TriggerJoyPadWhenAxisIsPressed() {
    //Strength by default is 1!
    Input.ActionPress("camera_right");
    Input.ActionPress("camera_up");

    var xMotion = InputUtilities.GetJoyPadActionPressedMotion(
      "camera_left", "camera_right", JoyAxis.RightX
    );

    var yMotion = InputUtilities.GetJoyPadActionPressedMotion(
      "camera_up", "camera_down", JoyAxis.RightY
    );

    xMotion!.AxisValue.ShouldBe(1);
    yMotion!.AxisValue.ShouldBe(-1);
  }

  [Test]
  public void NotTriggerJoyPadWhenAxisIsPressedInDeadZone() {
    Input.ActionPress("camera_right", _deadZoneX - 0.05f);
    Input.ActionPress("camera_up", _deadZoneY - 0.05f);

    var xMotion = InputUtilities.GetJoyPadActionPressedMotion(
      "camera_left", "camera_right", JoyAxis.RightX
    );

    var yMotion = InputUtilities.GetJoyPadActionPressedMotion(
      "camera_up", "camera_down", JoyAxis.RightY
    );

    xMotion.ShouldBeNull();
    yMotion.ShouldBeNull();
  }
}
