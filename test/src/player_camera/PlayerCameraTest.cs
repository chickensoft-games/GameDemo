namespace GameDemo.Tests;

using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Chickensoft.AutoInject;
using Chickensoft.GodotNodeInterfaces;
using Chickensoft.GoDotTest;
using Chickensoft.GodotTestDriver;
using Chickensoft.LogicBlocks;
using Chickensoft.Sync.Primitives;
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
  private LogicBlock.FakeBinding _binding = default!;
  private PlayerCameraSettings _settings = default!;
  private Mock<IGameRepo> _gameRepo = default!;
  private Mock<IAppRepo> _appRepo = default!;

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
    _binding = LogicBlock.CreateFakeBinding();
    _settings = new();
    _gameRepo = new();
    _appRepo = new();

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

    _gameRepo.Setup(repo => repo.IsMouseCaptured).Returns(new AutoValue<bool>(false));
    _gameRepo.Setup(repo => repo.PlayerGlobalPosition).Returns(new AutoValue<Vector3>(Vector3.Zero));

    _playerCam.FakeDependency(_gameRepo.Object);
    _playerCam.FakeDependency(_appRepo.Object);

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
        in It.Ref<PlayerCameraLogicState.Input.PhysicsTicked>.IsAny
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
        in It.Ref<PlayerCameraLogicState.Input.MouseInputOccurred>.IsAny
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
      new PlayerCameraLogicState.Output.GimbalRotationChanged(
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
      new PlayerCameraLogicState.Output.GlobalTransformChanged(transform)
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
      new PlayerCameraLogicState.Output.CameraLocalPositionChanged(value)
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
      new PlayerCameraLogicState.Output.CameraOffsetChanged(value)
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
  public async Task Saves()
  {
    // This test has to be run in the actual scene tree since it verifies the
    // player camera changes its GlobalPosition.
    var tree = TestScene.GetTree();
    var fixture = new Fixture(tree);
    await fixture.AddToRoot(_playerCam);

    var logic = new PlayerCameraLogic();
    logic.Set(_gameRepo.Object);
    logic.Set<IPlayerCamera>(_playerCam);
    logic.Set(_settings);
    logic.Set(new PlayerCameraLogic.Data
    {
      TargetPosition = Vector3.Zero,
      TargetAngleHorizontal = 0f,
      TargetAngleVertical = 0f,
      TargetOffset = Vector3.Zero
    });

    logic.Start<PlayerCameraLogicState.InputDisabled>();

    _playerCam.GlobalTransform = Transform3D.FlipZ;
    _playerCam.CameraLogic = logic;
    _cameraNode.Setup(n => n.Position).Returns(Vector3.Right);
    _offsetNode.Setup(n => n.Position).Returns(Vector3.Left);

    var data = _playerCam.Save();

    data.GlobalTransform.ShouldBe(Transform3D.FlipZ);
    data.StateMachine.Data.ShouldBe(logic.GetSaveData().Data);
    data.LocalPosition.ShouldBe(Vector3.Right);
    data.OffsetPosition.ShouldBe(Vector3.Left);
  }

  [Test]
  public async Task Loads()
  {
    // This test has to be run in the actual scene tree since it verifies the
    // player camera changes its GlobalPosition.
    var tree = TestScene.GetTree();
    var fixture = new Fixture(tree);
    await fixture.AddToRoot(_playerCam);

    _playerCam.Setup();

    var logic = new PlayerCameraLogic();
    logic.Set(_gameRepo.Object);
    logic.Set<IPlayerCamera>(_playerCam);
    logic.Set(_settings);
    logic.Set(new PlayerCameraLogic.Data
    {
      TargetPosition = Vector3.Zero,
      TargetAngleHorizontal = 0f,
      TargetAngleVertical = 0f,
      TargetOffset = Vector3.Zero
    });

    logic.Start<PlayerCameraLogicState.InputDisabled>();

    var data = new PlayerCameraData()
    {
      GlobalTransform = Transform3D.FlipZ,
      StateMachine = logic.GetSaveData(),
      LocalPosition = Vector3.Right,
      OffsetPosition = Vector3.Left,
    };

    _playerCam.CameraLogic = logic;
    _playerCam.Load(data);

    _playerCam.GlobalTransform.ShouldBe(Transform3D.FlipZ);
    _playerCam.CameraLogic.GetData().ShouldBe(data.StateMachine.Data);
    _cameraNode.VerifySet(n => n.Position = Vector3.Right);
    _offsetNode.VerifySet(n => n.Position = Vector3.Left);
  }
}
