namespace GameDemo.Tests;

using Chickensoft.GoDotCollections;
using Chickensoft.GoDotTest;
using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;
using Godot;
using Moq;
using Shouldly;

public partial class PlayerLogicStateAliveTest : TestClass {
  [Meta, TestState]
  public partial record TestPlayerState : PlayerLogic.State.Alive;

  private IFakeContext _context = default!;
  private PlayerLogic.Data _data = default!;
  private Mock<IPlayer> _player = default!;
  private PlayerLogic.Settings _settings = default!;
  private Mock<IGameRepo> _gameRepo = default!;
  private Mock<IAppRepo> _appRepo = default!;
  private PlayerLogic.State.Alive _state = default!;

  public PlayerLogicStateAliveTest(Node testScene) :
    base(testScene) {
  }

  [Setup]
  public void Setup() {
    _data = new();
    _player = new Mock<IPlayer>();
    _settings = new PlayerLogic.Settings(1, 1, 1, 1, 1, 1, 1);
    _gameRepo = new Mock<IGameRepo>();
    _appRepo = new Mock<IAppRepo>();

    _state = new TestPlayerState();
    _context = _state.CreateFakeContext();

    _context.Set(_data);
    _context.Set(_player.Object);
    _context.Set(_settings);
    _context.Set(_gameRepo.Object);
    _context.Set(_appRepo.Object);
  }

  [Test]
  public void Dies() {
    _gameRepo.Setup(repo => repo.OnGameEnded(GameOverReason.Lost));

    var next = _state.On(new PlayerLogic.Input.Killed());

    _gameRepo.VerifyAll();
    next.State.ShouldBeOfType<PlayerLogic.State.Dead>();
  }

  [Test]
  public void PhysicsUpdatesLastStrongDirection() {
    var input = new PlayerLogic.Input.PhysicsTick(1);

    var cameraBasis = new AutoProp<Basis>(Basis.Identity);
    _gameRepo.Setup(repo => repo.CameraBasis).Returns(cameraBasis);
    _player
      .Setup(player => player.GetGlobalInputVector(It.IsAny<Basis>()))
      .Returns(Vector3.Forward * 10);
    _player
      .Setup(player => player.GetNextRotationBasis(
        It.IsAny<Vector3>(), It.IsAny<double>(), It.IsAny<float>()
      ))
      .Returns(Basis.Identity);

    _state.On(input);

    _data.LastStrongDirection.ShouldBe(Vector3.Forward);
  }

  [Test]
  public void PhysicsStopsIfVelocityIsLessThanStoppingSpeed() {
    var input = new PlayerLogic.Input.PhysicsTick(1);

    var cameraBasis = new AutoProp<Basis>(Basis.Identity);
    _gameRepo.Setup(repo => repo.CameraBasis).Returns(cameraBasis);
    _player
      .Setup(player => player.GetGlobalInputVector(It.IsAny<Basis>()))
      .Returns(Vector3.Zero);
    _player
      .Setup(player => player.GetNextRotationBasis(
        It.IsAny<Vector3>(), It.IsAny<double>(), It.IsAny<float>()
      ))
      .Returns(Basis.Identity);

    _state.On(input);

    _context.Outputs.ShouldBe(new object[] {
      new PlayerLogic.Output.MovementComputed(
        Basis.Identity, Vector3.Up
      )
    });
  }

  [Test]
  public void MovedInputsHitFloor() {
    var input = new PlayerLogic.Input.Moved();

    _gameRepo.Setup(repo => repo.SetPlayerGlobalPosition(It.IsAny<Vector3>()));
    _player.Setup(player => player.Velocity).Returns(Vector3.Zero);
    _player.Setup(player => player.IsMovingHorizontally()).Returns(false);
    _player.Setup(player => player.IsOnFloor()).Returns(true);

    _data.WasOnFloor = false;
    _data.LastVelocity = Vector3.Down; // player has been falling

    // these conditions should trigger justHitFloor

    _state.On(input);

    _context.Inputs.ShouldBe(new object[] {
      new PlayerLogic.Input.HitFloor(IsMovingHorizontally: false)
    });
  }

  [Test]
  public void MovedInputsLeftFloor() {
    var input = new PlayerLogic.Input.Moved();

    _gameRepo.Setup(repo => repo.SetPlayerGlobalPosition(It.IsAny<Vector3>()));
    _player.Setup(player => player.Velocity).Returns(Vector3.Up); // jump
    _player.Setup(player => player.IsMovingHorizontally()).Returns(false);
    _player.Setup(player => player.IsOnFloor()).Returns(false);

    _data.WasOnFloor = true;
    _data.LastVelocity = Vector3.Zero; // player was still

    // these conditions should trigger justLeftFloor

    _state.On(input);

    _context.Inputs.ShouldBe(new object[] {
      new PlayerLogic.Input.LeftFloor(IsFalling: false)
    });
  }

  [Test]
  public void MovedInputsStartedFalling() {
    var input = new PlayerLogic.Input.Moved();

    _gameRepo.Setup(repo => repo.SetPlayerGlobalPosition(It.IsAny<Vector3>()));
    _player.Setup(player => player.Velocity).Returns(Vector3.Down); // fall
    _player.Setup(player => player.IsMovingHorizontally()).Returns(false);
    _player.Setup(player => player.IsOnFloor()).Returns(true);

    _data.WasOnFloor = true;
    _data.LastVelocity = Vector3.Zero; // player was still

    // these conditions should trigger justStartedFalling

    _state.On(input);

    _context.Inputs.ShouldBe(new object[] {
      new PlayerLogic.Input.StartedFalling()
    });
  }

  [Test]
  public void MovedInputsStartedMovingHorizontally() {
    var input = new PlayerLogic.Input.Moved();

    _gameRepo.Setup(repo => repo.SetPlayerGlobalPosition(It.IsAny<Vector3>()));
    _player.Setup(player => player.Velocity).Returns(Vector3.Right); // move
    _player.Setup(player => player.IsMovingHorizontally()).Returns(true);
    _player.Setup(player => player.IsOnFloor()).Returns(true);

    _data.WasOnFloor = true;
    _data.LastVelocity = Vector3.Zero; // player was still

    // these conditions should trigger justStartedMovingHorizontally

    _state.On(input);

    _context.Inputs.ShouldBe(new object[] {
      new PlayerLogic.Input.StartedMovingHorizontally()
    });
  }

  [Test]
  public void MovedInputsStoppedMovingHorizontally() {
    var input = new PlayerLogic.Input.Moved();

    _gameRepo.Setup(repo => repo.SetPlayerGlobalPosition(It.IsAny<Vector3>()));
    _player.Setup(player => player.Velocity).Returns(Vector3.Zero); // stop
    _player.Setup(player => player.IsMovingHorizontally()).Returns(false);
    _player.Setup(player => player.IsOnFloor()).Returns(true);

    _data.WasOnFloor = true;
    _data.LastVelocity = Vector3.Right * 10; // player was moving horizontally

    // these conditions should trigger justStoppedMovingHorizontally

    _state.On(input);

    _context.Inputs.ShouldBe(new object[] {
      new PlayerLogic.Input.StoppedMovingHorizontally()
    });
  }

  [Test]
  public void PushedChangesVelocity() {
    var input = new PlayerLogic.Input.Pushed(
      GlobalForceImpulseVector: Vector3.Forward * 10
    );

    _player.Setup(player => player.Velocity).Returns(Vector3.Zero);

    _state.On(input);

    _context.Outputs.ShouldBe(new object[] {
      new PlayerLogic.Output.VelocityChanged(Vector3.Forward * 10)
    });
  }
}
