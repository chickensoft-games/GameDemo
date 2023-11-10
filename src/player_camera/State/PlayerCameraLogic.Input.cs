namespace GameDemo;
using Godot;

public partial class PlayerCameraLogic {
  public static class Input {
    public readonly record struct TargetOffsetChanged(Vector3 TargetOffset);
    public readonly record struct PhysicsTicked(double Delta);
    public readonly record struct MouseInputOccurred(
      InputEventMouseMotion Motion
    );
    public readonly record struct TargetPositionChanged(Vector3 TargetPosition);
    public readonly record struct EnableInput;
    public readonly record struct DisableInput;
  }
}
