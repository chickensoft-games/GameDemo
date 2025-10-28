namespace GameDemo.Tests;

using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Chickensoft.AutoInject;
using Chickensoft.Collections;
using Chickensoft.GodotNodeInterfaces;
using Chickensoft.GoDotTest;
using Chickensoft.GodotTestDriver;
using Chickensoft.SaveFileBuilder;
using Godot;
using Moq;
using Shouldly;

[
  SuppressMessage(
    "Design",
    "CA1001",
    Justification = "Disposable fields are Godot objects; Godot will dispose"
  )
]
public class PlayerCameraTest : TestClass
{
  private PlayerCamera _playerCam = default!;
  private Mock<IPlayerCameraLogic> _logic = default!;
  private PlayerCameraLogic.IFakeBinding _binding = default!;
  private PlayerCameraSettings _settings = default!;
  private Mock<IGameRepo> _gameRepo = default!;
  private Mock<IAppRepo> _appRepo = default!;
  private Mock<ISaveChunk<GameData>> _gameChunk = default!;

  private Mock<INode3D> _offsetNode = default!;
  private Mock<INode3D> _gimbalHorizontal = default!;
  private Mock<INode3D> _gimbalVertical = default!;
  private Mock<ICamera3D> _cameraNode = default!;
  private Mock<INode3D> _springArmTarget = default!;

  public PlayerCameraTest(Node testScene) :
    base(testScene)
  { }

  [Setup]
  public void Setup()
  {
    _logic = new();
    _binding = PlayerCameraLogic.CreateFakeBinding();
    _settings = new();
    _gameRepo = new();
    _appRepo = new();
    _gameChunk = new();

    _offsetNode = new();
    _gimbalHorizontal = new();
    _gimbalVertical = new();
    _cameraNode = new();
    _springArmTarget = new();

    _playerCam = new()
    {
      OffsetNode = _offsetNode.Object,
      GimbalHorizontalNode = _gimbalHorizontal.Object,
      GimbalVerticalNode = _gimbalVertical.Object,
      CameraNode = _cameraNode.Object,
      SpringArmTarget = _springArmTarget.Object,
      CameraLogic = _logic.Object,
      Settings = _settings
    };

    (_playerCam as IAutoInit).IsTesting = true;

    _playerCam.FakeNodeTree(new()
    {
      ["%Offset"] = _offsetNode.Object,
      ["%GimbalHorizontal"] = _gimbalHorizontal.Object,
      ["%GimbalVertical"] = _gimbalVertical.Object,
      ["%Camera3D"] = _cameraNode.Object,
      ["%SpringArmTarget"] = _springArmTarget.Object
    });

    _playerCam.FakeDependency(_gameRepo.Object);
    _playerCam.FakeDependency(_appRepo.Object);
    _playerCam.FakeDependency(_gameChunk.Object);

    _logic.Setup(logic => logic.Bind()).Returns(_binding);
  }

  [Test]
  public void Initializes()
  {
    _playerCam.Setup();
    _playerCam.CameraLogic.ShouldBeOfType<PlayerCameraLogic>();
  }

  [Test]
  public void OnPhysicsProcess()
  {
    _logic.Reset();

    Input.ActionPress("camera_left", 0.8f);
    Input.ActionPress("camera_up", 0.8f);
    Input.ActionPress("camera_right", 0f);
    Input.ActionPress("camera_down", 0f);

    _logic.Setup(
      logic => logic.Input(
        in It.Ref<PlayerCameraLogic.Input.PhysicsTicked>.IsAny
      )
    );
    _playerCam.OnPhysicsProcess(1d);

    _logic.VerifyAll();
  }

  [Test]
  public void InputsMouseMotion()
  {
    _logic.Reset();

    var motion = new InputEventMouseMotion();
    _logic.Setup(
      logic => logic.Input(
        in It.Ref<PlayerCameraLogic.Input.MouseInputOccurred>.IsAny
      )
    );

    _playerCam._Input(motion);

    _logic.VerifyAll();
  }

  [Test]
  public void Getters()
  {
    _springArmTarget.Setup(node => node.Position).Returns(Vector3.Up);
    _playerCam.SpringArmTargetPosition.ShouldBe(Vector3.Up);

    _cameraNode.Setup(node => node.Position).Returns(Vector3.Up);
    _playerCam.CameraLocalPosition.ShouldBe(Vector3.Up);

    _gimbalHorizontal.Setup(node => node.Rotation).Returns(Vector3.Up);
    _playerCam.GimbalRotationHorizontal.ShouldBe(Vector3.Up);

    _gimbalVertical.Setup(node => node.Rotation).Returns(Vector3.Up);
    _playerCam.GimbalRotationVertical.ShouldBe(Vector3.Up);

    _gimbalHorizontal.Setup(node => node.GlobalTransform)
      .Returns(Transform3D.Identity);
    _playerCam.CameraBasis.ShouldBe(Transform3D.Identity.Basis);

    _offsetNode.Setup(node => node.Position).Returns(Vector3.Up);
    _playerCam.OffsetPosition.ShouldBe(Vector3.Up);
  }

  [Test]
  public void UpdatesGimbalRotation()
  {
    _gimbalHorizontal.SetupSet(node => node.Rotation = Vector3.Up);
    _gimbalVertical.SetupSet(node => node.Rotation = Vector3.Up);

    _playerCam.OnResolved();

    _binding.Output(
      new PlayerCameraLogic.Output.GimbalRotationChanged(
        Vector3.Up, Vector3.Up
      )
    );

    _gimbalHorizontal.VerifyAll();
    _gimbalVertical.VerifyAll();
  }

  [Test]
  public async Task UpdatesGlobalTransform()
  {
    // This test has to be conducted inside the scene tree so that we can
    // verify GlobalTransform is updated.
    var fixture = new Fixture(TestScene.GetTree());
    await fixture.AddToRoot(_playerCam);

    var transform =
      new Transform3D(Vector3.Up, Vector3.Up, Vector3.Up, Vector3.Zero);

    _playerCam.OnResolved();

    _binding.Output(
      new PlayerCameraLogic.Output.GlobalTransformChanged(transform)
    );

    _playerCam.GlobalTransform.ShouldBe(transform);

    await fixture.Cleanup();
  }

  [Test]
  public void UpdatesCameraLocalPosition()
  {
    var value = Vector3.Up;
    _cameraNode.SetupSet(node => node.Position = value);
    _playerCam.OnResolved();

    _binding.Output(
      new PlayerCameraLogic.Output.CameraLocalPositionChanged(value)
    );

    _cameraNode.VerifyAll();
  }

  [Test]
  public void UpdatesCameraOffset()
  {
    var value = Vector3.Up;
    _offsetNode.SetupSet(node => node.Position = value);
    _playerCam.OnResolved();

    _binding.Output(
      new PlayerCameraLogic.Output.CameraOffsetChanged(value)
    );

    _offsetNode.VerifyAll();
  }

  [Test]
  public void MakesItselfCurrentCamera()
  {
    _cameraNode.Setup(node => node.MakeCurrent());

    _playerCam.UsePlayerCamera();

    _cameraNode.VerifyAll();
  }

  [Test]
  public void Saves()
  {
    _playerCam.Setup();

    var chunk = new Mock<ISaveChunk<PlayerCameraData>>();

    _playerCam.CameraLogic.Get<PlayerCameraLogic.Data>().ShouldNotBeNull();

    var data = _playerCam.PlayerCameraChunk.OnSave(chunk.Object);

    data.GlobalTransform.ShouldBe(_playerCam.GlobalTransform);
    data.StateMachine.ShouldBe(_playerCam.CameraLogic);
    data.LocalPosition.ShouldBe(_playerCam.CameraNode.Position);
    data.OffsetPosition.ShouldBe(_playerCam.OffsetNode.Position);
  }

  [Test]
  public void Loads()
  {
    _playerCam.Setup();

    var chunk = new Mock<ISaveChunk<PlayerCameraData>>();

    var logic = new PlayerCameraLogic();
    _gameRepo.Setup(g => g.IsMouseCaptured).Returns(new AutoProp<bool>(false));
    _gameRepo.Setup(g => g.PlayerGlobalPosition)
      .Returns(new AutoProp<Vector3>(Vector3.Zero));

    logic.Set(_gameRepo.Object);
    logic.Set(new PlayerCameraLogic.Data
    {
      TargetPosition = Vector3.Zero,
      TargetAngleHorizontal = 0f,
      TargetAngleVertical = 0f,
      TargetOffset = Vector3.Zero
    });

    logic.Start();

    var data = new PlayerCameraData
    {
      StateMachine = logic,
      GlobalTransform = Transform3D.Identity,
      LocalPosition = Vector3.Zero,
      OffsetPosition = Vector3.Zero
    };

    _playerCam.PlayerCameraChunk.OnLoad(chunk.Object, data);

    _playerCam.GlobalTransform.ShouldBe(Transform3D.Identity);
    _playerCam.CameraNode.Position.ShouldBe(Vector3.Zero);
    _playerCam.OffsetNode.Position.ShouldBe(Vector3.Zero);
  }
}
