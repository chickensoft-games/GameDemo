namespace GameDemo.Tests;

using System.Diagnostics.CodeAnalysis;
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
[Collection(Constants.Headless)]
public class PlayerCameraLogicStateTest : IDisposable
{
  private readonly Mock<IPlayerCamera> _camera = new();
  private readonly PlayerCameraSettings _settings = new();
  private readonly Mock<IGameRepo> _gameRepo = new();
  private readonly PlayerCameraLogic.Data _data = new()
  {
    TargetPosition = Vector3.Zero,
    TargetAngleHorizontal = 0,
    TargetAngleVertical = 0,
    TargetOffset = Vector3.Zero
  };
  private readonly StateTester _context;
  private readonly PlayerCameraLogicState _playerCameraLogicState = new PlayerCameraLogicState.InputEnabled();

  public PlayerCameraLogicStateTest()
  {
    _context = _playerCameraLogicState.Test();

    // Automatically mock the logic block context to provide mock versions
    // of everything the state needs.
    _context.Set(_camera.Object);
    _context.Set(_settings);
    _context.Set(_gameRepo.Object);
    _context.Set(_data);
  }

  public void Dispose() => _settings.Dispose();

  [Fact]
  public void OnCameraTargetOffsetChanged()
  {
    // Make sure it updates the camera offset when the player moves.
    _playerCameraLogicState.OnCameraTargetOffsetChanged(Vector3.Zero);

    _context.Inputs.ShouldBe([
      new PlayerCameraLogicState.Input.TargetOffsetChanged(Vector3.Zero)
    ]);
  }

  [Fact]
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

    var next = _playerCameraLogicState.On(new PlayerCameraLogicState.Input.PhysicsTicked(1d));

    _playerCameraLogicState.ShouldBeOfType(next);

    _context.Outputs.ShouldBeOfTypes(
      typeof(PlayerCameraLogicState.Output.GimbalRotationChanged),
      typeof(PlayerCameraLogicState.Output.GlobalTransformChanged),
      typeof(PlayerCameraLogicState.Output.CameraLocalPositionChanged),
      typeof(PlayerCameraLogicState.Output.CameraOffsetChanged));

    _camera.VerifyAll();
  }

  [Fact]
  public void OnTargetPositionChanged()
  {
    var newTargetPosition = Vector3.Up;

    _playerCameraLogicState.On(new PlayerCameraLogicState.Input.TargetPositionChanged(
      newTargetPosition
    ));

    _data.TargetPosition.ShouldBe(newTargetPosition);
  }

  [Fact]
  public void TargetOffsetChanged()
  {
    var newTargetOffset = Vector3.Up;

    _playerCameraLogicState.On(new PlayerCameraLogicState.Input.TargetOffsetChanged(
      newTargetOffset
    ));

    _data.TargetOffset.ShouldBe(newTargetOffset);
  }
}
