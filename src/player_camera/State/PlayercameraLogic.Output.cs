namespace GameDemo;

using Godot;

public partial class PlayerCameraLogic
{
  public static class Output
  {
    public readonly record struct GimbalRotationChanged(
      Vector3 GimbalRotationHorizontal, Vector3 GimbalRotationVertical
    );
    public readonly record struct GlobalTransformChanged(
      Transform3D GlobalTransform
    );
    public readonly record struct CameraLocalPositionChanged(
      Vector3 CameraLocalPosition
    );
    public readonly record struct CameraOffsetChanged(Vector3 Offset);
  }
}
