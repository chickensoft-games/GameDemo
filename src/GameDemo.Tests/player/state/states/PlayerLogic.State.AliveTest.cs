namespace GameDemo.Tests;

using Chickensoft.Introspection;
using Chickensoft.LogicBlocks.Auto;
using Chickensoft.Sync.Primitives;
using Godot;
using Moq;
using Shouldly;

public partial class PlayerLogicStateAliveTest
{
  [Meta, TestState]
  public partial record TestPlayerState : PlayerLogicState.Alive;

  private readonly StateTester _context;
  private readonly PlayerLogic.Data _data = new();
  private readonly Mock<IPlayer> _player = new();
  private readonly PlayerLogic.Settings _settings = new(1, 1, 1, 1, 1, 1, 1);
  private readonly Mock<IGameRepo> _gameRepo = new();
  private readonly Mock<IAppRepo> _appRepo = new();
  private readonly PlayerLogicState.Alive _state = new TestPlayerState();

  public PlayerLogicStateAliveTest()
  {
    _context = _state.Test();

    _context.Set(_data);
    _context.Set(_player.Object);
    _context.Set(_settings);
    _context.Set(_gameRepo.Object);
    _context.Set(_appRepo.Object);
  }

  [Fact]
  public void Dies()
  {
    _gameRepo.Setup(repo => repo.OnGameEnded(GameOverReason.Lost));

    var next = _state.On(new PlayerLogicState.Input.Killed());

    _gameRepo.VerifyAll();
    next.IsAssignableTo(typeof(PlayerLogicState.Dead)).ShouldBeTrue();
  }

  [Fact]
  public void PhysicsUpdatesLastStrongDirection()
  {
    var input = new PlayerLogicState.Input.PhysicsTick(1);

    var cameraBasis = new AutoValue<Basis>(Basis.Identity);
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

  [Fact]
  public void PhysicsStopsIfVelocityIsLessThanStoppingSpeed()
  {
    var input = new PlayerLogicState.Input.PhysicsTick(1);

    var cameraBasis = new AutoValue<Basis>(Basis.Identity);
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

    _context.Outputs.ShouldBe([
      new PlayerLogicState.Output.MovementComputed(
        Basis.Identity, Vector3.Up, Vector2.Up, 1d
      )
    ]);
  }

  [Fact]
  public void MovedInputsHitFloor()
  {
    var input = new PlayerLogicState.Input.Moved();

    _gameRepo.Setup(repo => repo.SetPlayerGlobalPosition(It.IsAny<Vector3>()));
    _player.Setup(player => player.Velocity).Returns(Vector3.Zero);
    _player.Setup(player => player.IsMovingHorizontally()).Returns(false);
    _player.Setup(player => player.IsOnFloor()).Returns(true);

    _data.WasOnFloor = false;
    _data.LastVelocity = Vector3.Down; // player has been falling

    // these conditions should trigger justHitFloor

    _state.On(input);

    _context.Inputs.ShouldBe([
      new PlayerLogicState.Input.HitFloor(IsMovingHorizontally: false)
    ]);
  }

  [Fact]
  public void MovedInputsLeftFloor()
  {
    var input = new PlayerLogicState.Input.Moved();

    _gameRepo.Setup(repo => repo.SetPlayerGlobalPosition(It.IsAny<Vector3>()));
    _player.Setup(player => player.Velocity).Returns(Vector3.Up); // jump
    _player.Setup(player => player.IsMovingHorizontally()).Returns(false);
    _player.Setup(player => player.IsOnFloor()).Returns(false);

    _data.WasOnFloor = true;
    _data.LastVelocity = Vector3.Zero; // player was still

    // these conditions should trigger justLeftFloor

    _state.On(input);

    _context.Inputs.ShouldBe([
      new PlayerLogicState.Input.LeftFloor(IsFalling: false)
    ]);
  }

  [Fact]
  public void MovedInputsStartedFalling()
  {
    var input = new PlayerLogicState.Input.Moved();

    _gameRepo.Setup(repo => repo.SetPlayerGlobalPosition(It.IsAny<Vector3>()));
    _player.Setup(player => player.Velocity).Returns(Vector3.Down); // fall
    _player.Setup(player => player.IsMovingHorizontally()).Returns(false);
    _player.Setup(player => player.IsOnFloor()).Returns(true);

    _data.WasOnFloor = true;
    _data.LastVelocity = Vector3.Zero; // player was still

    // these conditions should trigger justStartedFalling

    _state.On(input);

    _context.Inputs.ShouldBe([
      new PlayerLogicState.Input.StartedFalling()
    ]);
  }

  [Fact]
  public void MovedInputsStartedMovingHorizontally()
  {
    var input = new PlayerLogicState.Input.Moved();

    _gameRepo.Setup(repo => repo.SetPlayerGlobalPosition(It.IsAny<Vector3>()));
    _player.Setup(player => player.Velocity).Returns(Vector3.Right); // move
    _player.Setup(player => player.IsMovingHorizontally()).Returns(true);
    _player.Setup(player => player.IsOnFloor()).Returns(true);

    _data.WasOnFloor = true;
    _data.LastVelocity = Vector3.Zero; // player was still

    // these conditions should trigger justStartedMovingHorizontally

    _state.On(input);

    _context.Inputs.ShouldBe([
      new PlayerLogicState.Input.StartedMovingHorizontally()
    ]);
  }

  [Fact]
  public void MovedInputsStoppedMovingHorizontally()
  {
    var input = new PlayerLogicState.Input.Moved();

    _gameRepo.Setup(repo => repo.SetPlayerGlobalPosition(It.IsAny<Vector3>()));
    _player.Setup(player => player.Velocity).Returns(Vector3.Zero); // stop
    _player.Setup(player => player.IsMovingHorizontally()).Returns(false);
    _player.Setup(player => player.IsOnFloor()).Returns(true);

    _data.WasOnFloor = true;
    _data.LastVelocity = Vector3.Right * 10; // player was moving horizontally

    // these conditions should trigger justStoppedMovingHorizontally

    _state.On(input);

    _context.Inputs.ShouldBe([
      new PlayerLogicState.Input.StoppedMovingHorizontally()
    ]);
  }

  [Fact]
  public void PushedChangesVelocity()
  {
    var input = new PlayerLogicState.Input.Pushed(
      GlobalForceImpulseVector: Vector3.Forward * 10
    );

    _player.Setup(player => player.Velocity).Returns(Vector3.Zero);

    _state.On(input);

    _context.Outputs.ShouldBe([
      new PlayerLogicState.Output.VelocityChanged(Vector3.Forward * 10)
    ]);
  }
}
