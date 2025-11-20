namespace GameDemo.Tests;

using System.Diagnostics.CodeAnalysis;
using Chickensoft.GoDotTest;
using Chickensoft.LogicBlocks;
using Chickensoft.Sync.Primitives;
using Godot;
using Moq;
using Shouldly;

[
  SuppressMessage(
    "Design",
    "CA1001",
    Justification = "Disposable field is disposed in cleanup"
  )
]
public class PlayerCameraLogicStateTest : TestClass
{
  private Mock<IPlayerCamera> _camera = default!;
  private PlayerCameraSettings _settings = default!;
  private Mock<IGameRepo> _gameRepo = default!;
  private PlayerCameraLogic.Data _data = default!;
  private IFakeContext _context = default!;
  private PlayerCameraLogic.State _state = default!;

  public PlayerCameraLogicStateTest(Node testScene) : base(testScene) { }

  [Setup]
  public void Setup()
  {
    _camera = new();
    _settings = new();
    _gameRepo = new();
    _data = new()
    {
      TargetPosition = Vector3.Zero,
      TargetAngleHorizontal = 0,
      TargetAngleVertical = 0,
      TargetOffset = Vector3.Zero
    };
    _state = new PlayerCameraLogic.State.InputEnabled();
    _context = _state.CreateFakeContext();

    // Automatically mock the logic block context to provide mock versions
    // of everything the state needs.
    _context.Set(_camera.Object);
    _context.Set(_settings);
    _context.Set(_gameRepo.Object);
    _context.Set(_data);
  }

  [Cleanup]
  public void Cleanup() => _settings.Dispose();

  [Test]
  public void OnCameraTargetOffsetChanged()
  {
    // Make sure it updates the camera offset when the player moves.
    _state.OnCameraTargetOffsetChanged(Vector3.Zero);

    _context.Inputs.ShouldBe([
      new PlayerCameraLogic.Input.TargetOffsetChanged(Vector3.Zero)
    ]);
  }

  [Test]
  public void OnPhysicsTicked()
  {
    _camera.Setup(cam => cam.GimbalRotationHorizontal).Returns(Vector3.Zero);
    _camera.Setup(cam => cam.GimbalRotationVertical).Returns(Vector3.Zero);
    _camera.Setup(cam => cam.CameraBasis).Returns(Basis.Identity);
    _camera.Setup(cam => cam.GlobalTransform).Returns(Transform3D.Identity);
    _camera.Setup(cam => cam.SpringArmTargetPosition).Returns(Vector3.Zero);
    _camera.Setup(cam => cam.CameraLocalPosition).Returns(Vector3.Zero);
    _camera.Setup(cam => cam.OffsetPosition).Returns(Vector3.Zero);

    // If you wanted to be more precise with your tests, you could check the
    // actual math in the output objects.
    //
    // In this case, I'm happy with visually verifying the math by playing the
    // game and verifying the camera moves how I'd expect. I just want to make
    // sure the outputs occur in the order I expect, as well as ensuring that
    // each branch of code is run in automated way at least once.
    //
    // On another note, writing tests, however silly they may be, proves that
    // your code is modular enough to be tested, which ultimately makes the code
    // easier to maintain and build upon. We're just glad this test can even
    // exist ^-^

    _gameRepo.Setup(repo => repo.SetCameraBasis(It.IsAny<Basis>()));

    var next = _state.On(new PlayerCameraLogic.Input.PhysicsTicked(1d));

    _state.ShouldBeSameAs(next.State);

    _context.Outputs.ShouldBeOfTypes(
      typeof(PlayerCameraLogic.Output.GimbalRotationChanged),
      typeof(PlayerCameraLogic.Output.GlobalTransformChanged),
      typeof(PlayerCameraLogic.Output.CameraLocalPositionChanged),
      typeof(PlayerCameraLogic.Output.CameraOffsetChanged));

    _camera.VerifyAll();
  }

  [Test]
  public void OnTargetPositionChanged()
  {
    var newTargetPosition = Vector3.Up;

    _state.On(new PlayerCameraLogic.Input.TargetPositionChanged(
      newTargetPosition
    ));

    _data.TargetPosition.ShouldBe(newTargetPosition);
  }

  [Test]
  public void TargetOffsetChanged()
  {
    var newTargetOffset = Vector3.Up;

    _state.On(new PlayerCameraLogic.Input.TargetOffsetChanged(
      newTargetOffset
    ));

    _data.TargetOffset.ShouldBe(newTargetOffset);
  }
}
