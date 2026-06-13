namespace GameDemo;

public partial class PlayerLogic
{
  /// <summary>Player settings.</summary>
  /// <param name="RotationSpeed">Rotation speed (quaternions?/sec).</param>
  /// <param name="StoppingSpeed">Stopping velocity (meters/sec).</param>
  /// <param name="Gravity">Player gravity (meters/sec).</param>
  /// <param name="MoveSpeed">Player speed (meters/sec).</param>
  /// <param name="Acceleration">Player speed (meters^2/sec).</param>
  /// <param name="JumpImpulseForce">Jump initial impulse force.</param>
  /// <param name="JumpForce">Additional force added each physics tick while
  /// player is still pressing jump.</param>
  public record Settings(
    float RotationSpeed,
    float StoppingSpeed,
    float Gravity,
    float MoveSpeed,
    float Acceleration,
    float JumpImpulseForce,
    float JumpForce
  );
}
