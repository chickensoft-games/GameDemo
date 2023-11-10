namespace GameDemo.Tests;

using Chickensoft.GoDotTest;
using Godot;
using Moq;
using Shouldly;

public class PlayerCameraLogicTest : TestClass {
  private Mock<IPlayerCamera> _camera = default!;
  private PlayerCameraSettings _settings = default!;
  private Mock<IAppRepo> _appRepo = default!;
  private Mock<IGameRepo> _gameRepo = default!;
  private PlayerCameraLogic _logic = default!;

  public PlayerCameraLogicTest(Node testScene) : base(testScene) { }

  [Setup]
  public void Setup() {
    _camera = new();
    _appRepo = new();
    _gameRepo = new();
    _settings = new();
    _logic = new(_camera.Object, _settings, _appRepo.Object, _gameRepo.Object);
  }

  [Test]
  public void Initializes() {
    // Make sure the camera logic block sets up the blackboard with
    // everything the states will need to use.
    var data = new PlayerCameraLogic.Data();

    data.TargetPosition.ShouldBe(Vector3.Zero);
    data.TargetAngleHorizontal.ShouldBe(0f);
    data.TargetAngleVertical.ShouldBe(0f);
    data.TargetOffset.ShouldBe(Vector3.Zero);

    _logic.Get<IPlayerCamera>().ShouldBe(_camera.Object);
    _logic.Get<PlayerCameraSettings>().ShouldBe(_settings);
    _logic.Get<IGameRepo>().ShouldBe(_gameRepo.Object);
    _logic.Get<PlayerCameraLogic.Data>().ShouldNotBeNull();

    var context = PlayerCameraLogic.CreateFakeContext();
    context.Set(_appRepo.Object);
    context.Set(_gameRepo.Object);
    _logic
      .GetInitialState(context)
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
