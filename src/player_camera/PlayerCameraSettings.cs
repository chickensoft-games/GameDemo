namespace GameDemo;

using Godot;

public partial class PlayerCameraSettings : Resource {
  [Export(PropertyHint.Range, "0, 10, 0.01")]
  public float MouseSensitivity { get; set; } = 0.2f;

  /// <summary>
  /// Vertical gimbal angle maximum constraint (in degrees).
  /// </summary>
  [Export(PropertyHint.Range, "0, 89.9, 0.01")]
  public float VerticalMax { get; set; } = 45;

  /// <summary>
  /// Vertical gimbal angle minimum constraint (in degrees).
  /// </summary>
  [Export(PropertyHint.Range, "-89.9, -0.01, 0.01")]
  public float VerticalMin { get; set; } = -45;

  /// <summary>
  /// How fast the camera follows the target (units per second).
  /// </summary>
  [Export(PropertyHint.Range, "0, 100, 0.1")]
  public float FollowSpeed = 15f;

  /// <summary>
  /// How fast the camera follows the spring arm target (units per second).
  /// </summary>
  [Export(PropertyHint.Range, "0, 100, 0.1")]
  public float SpringArmAdjSpeed = 15f;

  /// <summary>
  /// How fast the camera offset moves to the target offset (units per second).
  /// </summary>
  [Export(PropertyHint.Range, "0, 100, 0.1")]
  public float OffsetAdjSpeed = 2f;

  /// <summary>
  /// Acceleration for rotation applied to vertical gimbal.
  /// </summary>
  [Export(PropertyHint.Range, "0, 100, 0.1")]
  public float VerticalRotationAcceleration = 6f;

  /// <summary>
  /// Acceleration for rotation applied to horizontal gimbal.
  /// </summary>
  [Export(PropertyHint.Range, "0, 100, 0.1")]
  public float HorizontalRotationAcceleration = 6f;
}
