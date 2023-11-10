namespace GameDemo.Tests;

using System;
using Chickensoft.GoDotCollections;
using Chickensoft.GoDotTest;
using Godot;
using Moq;
using Shouldly;

public class PlayerCameraLogicStateTest : TestClass {
  private Mock<IPlayerCamera> _camera = default!;
  private PlayerCameraSettings _settings = default!;
  private Mock<IAppRepo> _appRepo = default!;
  private Mock<IGameRepo> _gameRepo = default!;
  private PlayerCameraLogic.Data _data = default!;
  private PlayerCameraLogic.IFakeContext _context = default!;

  public PlayerCameraLogicStateTest(Node testScene) : base(testScene) { }

  [Setup]
  public void Setup() {
    _camera = new();
    _settings = new();
    _appRepo = new();
    _gameRepo = new();
    _data = new();
    _context = PlayerCameraLogic.CreateFakeContext();

    // Automatically mock the logic block context to provide mock versions
    // of everything the state needs.
    _context.Set(_camera.Object);
    _context.Set(_settings);
    _context.Set(_appRepo.Object);
    _context.Set(_gameRepo.Object);
    _context.Set(_data);
  }

  [Test]
  public void ListensToMouseAndPlayerPosition() {
    var isMouseCaptured = new Mock<IAutoProp<bool>>();
    var playerGlobalPosition = new Mock<IAutoProp<Vector3>>();

    _appRepo.Setup(repo => repo.IsMouseCaptured)
      .Returns(isMouseCaptured.Object);
    _gameRepo.Setup(repo => repo.PlayerGlobalPosition)
      .Returns(playerGlobalPosition.Object);

    var state = new PlayerCameraLogic.State(_context);

    state.Enter();

    _appRepo
      .VerifyAdd(repo => repo.IsMouseCaptured.Sync += state.OnMouseCaptured);
    _gameRepo
      .VerifyAdd(repo =>
        repo.PlayerGlobalPosition.Sync += state.OnPlayerGlobalPositionChanged
      );

    state.Exit();

    _appRepo
      .VerifyRemove(repo => repo.IsMouseCaptured.Sync -= state.OnMouseCaptured);
    _gameRepo
      .VerifyRemove(repo =>
        repo.PlayerGlobalPosition.Sync -= state.OnPlayerGlobalPositionChanged
      );
  }

  [Test]
  public void OnMouseCaptured() {
    var state = new PlayerCameraLogic.State(_context);

    // Make sure it enables input when mouse is captured.
    state.OnMouseCaptured(true);

    _context.Inputs.ShouldBe(new object[] {
      new PlayerCameraLogic.Input.EnableInput()
    });

    _context.Reset();

    // Make sure it disables input when mouse is not captured.
    state.OnMouseCaptured(false);

    _context.Inputs.ShouldBe(new object[] {
      new PlayerCameraLogic.Input.DisableInput()
    });
  }

  [Test]
  public void OnPlayerGlobalPositionChanged() {
    var state = new PlayerCameraLogic.State(_context);

    // Make sure it updates the camera position when the player moves.
    state.OnPlayerGlobalPositionChanged(Vector3.Zero);

    _context.Inputs.ShouldBe(new object[] {
      new PlayerCameraLogic.Input.TargetPositionChanged(Vector3.Zero)
    });
  }

  [Test]
  public void OnCameraTargetOffsetChanged() {
    var state = new PlayerCameraLogic.State(_context);

    // Make sure it updates the camera offset when the player moves.
    state.OnCameraTargetOffsetChanged(Vector3.Zero);

    _context.Inputs.ShouldBe(new object[] {
      new PlayerCameraLogic.Input.TargetOffsetChanged(Vector3.Zero)
    });
  }

  [Test]
  public void OnPhysicsTicked() {
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

    var state = new PlayerCameraLogic.State(_context);

    var nextState = state.On(new PlayerCameraLogic.Input.PhysicsTicked(1d));

    state.ShouldBeSameAs(nextState);

    _context.Outputs.ShouldBeOfTypes(new Type[] {
      typeof(PlayerCameraLogic.Output.GimbalRotationChanged),
      typeof(PlayerCameraLogic.Output.GlobalTransformChanged),
      typeof(PlayerCameraLogic.Output.CameraLocalPositionChanged),
      typeof(PlayerCameraLogic.Output.CameraOffsetChanged),
    });

    _camera.VerifyAll();
  }

  [Test]
  public void OnTargetPositionChanged() {
    var originalTargetPosition = _data.TargetPosition;
    var newTargetPosition = Vector3.Up;

    var state = new PlayerCameraLogic.State(_context);

    state.On(new PlayerCameraLogic.Input.TargetPositionChanged(
      newTargetPosition
    ));

    _data.TargetPosition.ShouldBe(newTargetPosition);
  }

  [Test]
  public void TargetOffsetChanged() {
    var originalTargetOffset = _data.TargetOffset;
    var newTargetOffset = Vector3.Up;

    var state = new PlayerCameraLogic.State(_context);

    state.On(new PlayerCameraLogic.Input.TargetOffsetChanged(
      newTargetOffset
    ));

    _data.TargetOffset.ShouldBe(newTargetOffset);
  }
}
