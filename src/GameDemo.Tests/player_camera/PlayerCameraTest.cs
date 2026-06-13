namespace GameDemo.Tests;

using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Chickensoft.AutoInject;
using Chickensoft.GodotNodeInterfaces;
using Chickensoft.LogicBlocks;
using Chickensoft.SaveFileBuilder;
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
[Collection(Constants.HEADLESS)]
public class PlayerCameraTest
{
  private readonly GodotHeadlessFixture _godot;
  private readonly PlayerCamera _playerCam;
  private readonly Mock<IPlayerCameraLogic> _logic = new();
  private readonly LogicBlock.FakeBinding _binding = LogicBlock.CreateFakeBinding();
  private readonly PlayerCameraSettings _settings = new();
  private readonly Mock<IGameRepo> _gameRepo = new();
  private readonly Mock<IAppRepo> _appRepo = new();
  private readonly Mock<ISaveChunk<GameData>> _gameChunk = new();

  private readonly Mock<INode3D> _offsetNode = new();
  private readonly Mock<INode3D> _gimbalHorizontal = new();
  private readonly Mock<INode3D> _gimbalVertical = new();
  private readonly Mock<ICamera3D> _cameraNode = new();
  private readonly Mock<INode3D> _springArmTarget = new();

  public PlayerCameraTest(GodotHeadlessFixture godot)
  {
    _godot = godot;

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
    _playerCam.FakeDependency(_gameChunk.Object);

    _logic.Setup(logic => logic.Bind()).Returns(_binding);
  }

  [Fact]
  public void Initializes()
  {
    _playerCam.Setup();
    _playerCam.CameraLogic.ShouldBeOfType<PlayerCameraLogic>();
  }

  [Fact]
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

  [Fact]
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

  [Fact]
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

  [Fact]
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

  [Fact]
  public void UpdatesGlobalTransform()
  {
    // This test has to be conducted inside the scene tree so that we can
    // verify GlobalTransform is updated.
    _godot.Tree.Root.AddChild(_playerCam);

    var transform =
      new Transform3D(Vector3.Up, Vector3.Up, Vector3.Up, Vector3.Zero);

    _playerCam.OnResolved();

    _binding.Output(
      new PlayerCameraLogicState.Output.GlobalTransformChanged(transform)
    );

    _playerCam.GlobalTransform.ShouldBe(transform);

    _playerCam.QueueFree();
  }

  [Fact]
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

  [Fact]
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

  [Fact]
  public void MakesItselfCurrentCamera()
  {
    _cameraNode.Setup(node => node.MakeCurrent());

    _playerCam.UsePlayerCamera();

    _cameraNode.VerifyAll();
  }

  [Fact]
  public void Saves()
  {
    _playerCam.Setup();
    _playerCam.CameraLogic.Start<PlayerCameraLogicState.InputDisabled>();

    var chunk = new Mock<ISaveChunk<PlayerCameraData>>();

    _playerCam.CameraLogic.Get<PlayerCameraLogic.Data>().ShouldNotBeNull();

    var data = _playerCam.PlayerCameraChunk.OnSave(chunk.Object);

    data.GlobalTransform.ShouldBe(_playerCam.GlobalTransform);
    data.StateMachine.Data.ShouldBe(_playerCam.CameraLogic.Save().Data);
    data.LocalPosition.ShouldBe(_playerCam.CameraNode.Position);
    data.OffsetPosition.ShouldBe(_playerCam.OffsetNode.Position);
  }

  [Fact]
  public void Loads()
  {
    _playerCam.Setup();

    var chunk = new Mock<ISaveChunk<PlayerCameraData>>();

    var logic = new PlayerCameraLogic();
    _gameRepo.Setup(g => g.IsMouseCaptured).Returns(new AutoValue<bool>(false));
    _gameRepo.Setup(g => g.PlayerGlobalPosition)
      .Returns(new AutoValue<Vector3>(Vector3.Zero));

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
    _playerCam.CameraLogic = logic;

    var data = new PlayerCameraData
    {
      StateMachine = logic.Save(),
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
