namespace GameDemo;

using System;
using Godot;

public abstract class InputExtensions {
  public static InputEventJoypadMotion? GetJoyPadActionPressedMotion(StringName negativeAction, StringName positiveAction, JoyAxis axis, int device = 0) {
    var axisValue = Input.GetAxis(negativeAction, positiveAction);
    var isInDeadZone = Math.Abs(axisValue) < InputMap.ActionGetDeadzone(negativeAction) ||
                       Math.Abs(axisValue) < InputMap.ActionGetDeadzone(positiveAction);
    if (isInDeadZone) {
      return default;
    }

    if (Input.IsActionPressed(negativeAction) || Input.IsActionPressed(positiveAction)) {
      var motion = new InputEventJoypadMotion() {
        Axis = axis,
        AxisValue = axisValue,
        Device = device
      };

      return motion;
    }
    return default;
  }
}
