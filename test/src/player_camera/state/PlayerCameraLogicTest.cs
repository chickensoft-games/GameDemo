namespace GameDemo.Tests;

using Chickensoft.GoDotTest;
using Chickensoft.Sync.Primitives;
using Godot;
using Moq;
using Shouldly;

public class PlayerCameraLogicTest : TestClass
{
  private PlayerCameraLogic _logic = default!;
  private AutoValue<bool> _isMouseCaptured = default!;
  private AutoValue<Vector3> _playerGlobalPosition = default!;
  private PlayerCameraSettings _settings = default!;
  private Mock<IGameRepo> _gameRepo = default!;
  private Mock<IPlayerCamera> _camera = default!;
  public PlayerCameraLogicTest(Node testScene) : base(testScene) { }

  [Setup]
  public void Setup()
  {
    _logic = new();
    _settings = new();
    _gameRepo = new();
    _isMouseCaptured = new AutoValue<bool>(false);
    _playerGlobalPosition = new AutoValue<Vector3>(Vector3.Zero);
    _camera = new();

    _gameRepo.Setup(repo => repo.IsMouseCaptured).Returns(_isMouseCaptured);
    _gameRepo.Setup(repo => repo.PlayerGlobalPosition).Returns(_playerGlobalPosition);

    _logic.Set(_camera.Object);
    _logic.Set(_gameRepo.Object);
    _logic.Set(_settings);
    _logic.Set(new PlayerCameraLogic.Data
    {
      TargetPosition = Vector3.Zero,
      TargetAngleHorizontal = 0f,
      TargetAngleVertical = 0f,
      TargetOffset = Vector3.Zero
    });
  }

  [Test]
  public void Initializes()
  {
    // Make sure the camera logic block sets up the blackboard with
    // everything the states will need to use.
    var data = new PlayerCameraLogic.Data()
    {
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

  [Test]
  public void SubscribesToMouseCaptured()
  {
    _logic.Start();

    _logic.Value.ShouldBeOfType<PlayerCameraLogic.State.InputDisabled>();

    _isMouseCaptured.Value = true;
    _logic.Value.ShouldBeOfType<PlayerCameraLogic.State.InputEnabled>();

    _isMouseCaptured.Value = false;
    _logic.Value.ShouldBeOfType<PlayerCameraLogic.State.InputDisabled>();
  }

  [Test]
  public void SubscribesToPlayerGlobalPosition()
  {
    _logic.Start();

    var targetPos = new Vector3(10, 0, 10);
    _playerGlobalPosition.Value = targetPos;

    _camera.Setup(c => c.GimbalRotationHorizontal).Returns(Vector3.Zero);
    _camera.Setup(c => c.GimbalRotationVertical).Returns(Vector3.Zero);
    _camera.Setup(c => c.GlobalTransform).Returns(Transform3D.Identity);
    _camera.Setup(c => c.CameraLocalPosition).Returns(Vector3.Zero);
    _camera.Setup(c => c.OffsetPosition).Returns(Vector3.Zero);
    _camera.Setup(c => c.SpringArmTargetPosition).Returns(Vector3.Zero);
    _camera.Setup(c => c.Offset).Returns(Vector3.Zero);
    _camera.Setup(c => c.CameraBasis).Returns(Basis.Identity);

    using var binding = _logic.Bind();
    PlayerCameraLogic.Output.GlobalTransformChanged? lastOutput = null;

    binding.Handle(
      (in PlayerCameraLogic.Output.GlobalTransformChanged o) => lastOutput = o
    );

    _logic.Input(new PlayerCameraLogic.Input.PhysicsTicked(1.0));

    lastOutput.ShouldNotBeNull();
    lastOutput.Value.GlobalTransform.Origin.ShouldNotBe(Vector3.Zero);
  }

  [Test]
  public void OnStopWithoutStartSucceeds()
  {
    var logic = new PlayerCameraLogic();
    logic.OnStop();
  }

  [Cleanup]
  public void Cleanup()
  {
    _logic.Stop();
    _isMouseCaptured.Dispose();
    _playerGlobalPosition.Dispose();
  }
}
