namespace GameDemo;
using Godot;

public partial class PlayerCameraLogic {
  /// <summary>Player camera data.</summary>
  public record Data {
    /// <summary>
    /// The camera's target position. The camera will move towards this
    /// position each tick, allowing for smooth camera follow motion.
    /// </summary>
    public Vector3 TargetPosition { get; set; } = Vector3.Zero;
    /// <summary>Target horizontal angle (determined by input).</summary>
    public float TargetAngleHorizontal { get; set; }
    /// <summary>Target vertical angle (determined by input).</summary>
    public float TargetAngleVertical { get; set; }
    /// <summary>
    /// Target offset (used to slide the camera when strafing.
    /// </summary>
    public Vector3 TargetOffset { get; set; } = Vector3.Zero;
  }
}
