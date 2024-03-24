namespace GameDemo;

using System;
using Godot;

public abstract class InputUtilities {

  /// <summary>
  /// Get axis pressed input by specifying two actions,
  /// one negative and one positive.
  /// </summary>
  /// <param name="negativeAction"></param>
  /// <param name="positiveAction"></param>
  /// <param name="axis"></param>
  /// <param name="device"></param>
  /// <returns> A new <see cref="InputEventJoypadMotion"/> or
  /// <see cref="null"/> if AxisValue is in the DeadZone </returns>
  public static InputEventJoypadMotion? GetJoyPadActionPressedMotion(
    StringName negativeAction, StringName positiveAction,
    JoyAxis axis, int device = 0) {
    var axisValue = Input.GetAxis(negativeAction, positiveAction);
    var isInDeadZone =
      Math.Abs(axisValue) < InputMap.ActionGetDeadzone(negativeAction) ||
      Math.Abs(axisValue) < InputMap.ActionGetDeadzone(positiveAction);
    if (isInDeadZone && axisValue != 0) {
      return default;
    }

    if (Input.IsActionPressed(negativeAction) ||
      Input.IsActionPressed(positiveAction)) {
      var motion = new InputEventJoypadMotion() {
        Axis = axis,
        AxisValue = axisValue,
        Device = device
      };

      return motion;
    }

    return null;
  }
}
