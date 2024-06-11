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
    _deadZoneY = InputMap.ActionGetDeadzone("camera_down");
  }

  [Test]
  public void NotTriggerJoyPadWhenAxisIsNotPressed() {
    Input.UseAccumulatedInput = false;

    Input.ActionRelease("camera_left");
    Input.ActionRelease("camera_right");

    var xMotion = InputUtilities.GetJoyPadActionPressedMotion(
      "camera_left", "camera_right", JoyAxis.RightX
    );

    Input.ActionRelease("camera_up");
    Input.ActionRelease("camera_down");
    var yMotion = InputUtilities.GetJoyPadActionPressedMotion(
      "camera_up", "camera_down", JoyAxis.RightY
    );

    xMotion.ShouldBeNull();
    yMotion.ShouldBeNull();
  }

  [Test]
  public void TriggerJoyPadWhenAxisIsPressed() {
    //up and left are always negative
    //down and right are always positive

    Input.ActionPress("camera_right", 0.8f);
    var xMotion = InputUtilities.GetJoyPadActionPressedMotion(
      "camera_left", "camera_right", JoyAxis.RightX
    );
    xMotion?.AxisValue.ShouldBe(0.8f);

    Input.ActionPress("camera_down", 0.6f);
    System.Console.WriteLine($"Worked? {Input.GetActionStrength("camera_up")}");
    var yMotion = InputUtilities.GetJoyPadActionPressedMotion(
      "camera_up", "camera_down", JoyAxis.RightY
    );
    yMotion?.AxisValue.ShouldBe(0.6f);
  }

  [Test]
  public void NotTriggerJoyPadWhenAxisIsPressedInDeadZone() {
    Input.ActionRelease("camera_left");
    Input.ActionPress("camera_right", _deadZoneX - 0.1f);

    var xMotion = InputUtilities.GetJoyPadActionPressedMotion(
      "camera_left", "camera_right", JoyAxis.RightX
    );
    Input.ActionRelease("camera_right");

    Input.ActionRelease("camera_up");
    Input.ActionPress("camera_down", _deadZoneY - 0.1f);
    var yMotion = InputUtilities.GetJoyPadActionPressedMotion(
      "camera_up", "camera_down", JoyAxis.RightY
    );
    Input.ActionRelease("camera_down");

    xMotion.ShouldBeNull();
    yMotion.ShouldBeNull();
  }
}
