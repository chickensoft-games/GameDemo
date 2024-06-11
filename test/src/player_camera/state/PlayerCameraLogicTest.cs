namespace GameDemo.Tests;

using Chickensoft.GoDotTest;
using Godot;
using Shouldly;

public class PlayerCameraLogicTest : TestClass {
  private PlayerCameraLogic _logic = default!;

  public PlayerCameraLogicTest(Node testScene) : base(testScene) { }

  [Setup]
  public void Setup() {
    _logic = new();
  }

  [Test]
  public void Initializes() {
    // Make sure the camera logic block sets up the blackboard with
    // everything the states will need to use.
    var data = new PlayerCameraLogic.Data() {
      TargetPosition = Vector3.Zero,
      TargetAngleHorizontal = 0,
      TargetAngleVertical = 0,
      TargetOffset = Vector3.Zero
    };

    data.TargetPosition.ShouldBe(Vector3.Zero);
    data.TargetAngleHorizontal.ShouldBe(0f);
    data.TargetAngleVertical.ShouldBe(0f);
    data.TargetOffset.ShouldBe(Vector3.Zero);

    _logic
      .GetInitialState().State
      .ShouldBeOfType<PlayerCameraLogic.State.InputDisabled>();

    // Test outputs
    var gimbalRotationChanged =
      new PlayerCameraLogic.Output.GimbalRotationChanged(
        Vector3.Zero, Vector3.Zero
      );
    gimbalRotationChanged.GimbalRotationHorizontal.ShouldBe(Vector3.Zero);
    gimbalRotationChanged.GimbalRotationVertical.ShouldBe(Vector3.Zero);

    var globalTransformChanged =
      new PlayerCameraLogic.Output.GlobalTransformChanged(Transform3D.Identity);
    globalTransformChanged.GlobalTransform.ShouldBe(Transform3D.Identity);

    var cameraLocalPositionChanged =
      new PlayerCameraLogic.Output.CameraLocalPositionChanged(Vector3.Zero);
    cameraLocalPositionChanged.CameraLocalPosition.ShouldBe(Vector3.Zero);

    var cameraOffsetChanged =
      new PlayerCameraLogic.Output.CameraOffsetChanged(Vector3.Zero);
    cameraOffsetChanged.Offset.ShouldBe(Vector3.Zero);
  }
}
