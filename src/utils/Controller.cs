namespace GameDemo;

using System.Diagnostics.CodeAnalysis;
using Godot;

public static class Controller {
  public static string Current { get; set; } = "none";

  public static string ConfirmPath => Current.Contains("PS") ?
    "res://src/shared_ui/assets/PS4_Cross.png" : Current.Contains("XBox") ?
    "res://src/shared_ui/assets/XboxOne_A.png" :
    "res://src/shared_ui/assets/Space_Key_Light.png";

  /// <summary>
  /// Updates the Current Controller been used in real time!
  /// </summary>
  /// <param name="event"></param>
  [ExcludeFromCodeCoverage(
    Justification = "There is no method to mock Connected Controller"
  )]
  public static void HandleCurrentControllers(InputEvent @event) {
    if (@event is InputEventJoypadButton or InputEventJoypadMotion) {
      var devices = Input.GetConnectedJoypads();
      Current = Input.GetJoyName(devices.Count - 1);
    }
    else if (@event is InputEventKey) {
      Current = "none";
    }
  }
}
