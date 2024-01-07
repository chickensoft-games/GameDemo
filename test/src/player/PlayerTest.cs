namespace GameDemo.Tests;

using System;
using System.Threading.Tasks;
using Chickensoft.AutoInject;
using Chickensoft.GoDotTest;
using Chickensoft.LogicBlocks;
using Godot;
using GodotTestDriver;
using Moq;
using Shouldly;

public class PlayerTest : TestClass {
  private Fixture _fixture = default!;
  private Mock<IAppRepo> _appRepo = default!;
  private Mock<IGameRepo> _gameRepo = default!;
  private Mock<IPlayerLogic> _logic = default!;

  private Logic<PlayerLogic.IState, Func<object, PlayerLogic.IState>,
    PlayerLogic.IState, Action<PlayerLogic.IState?>>.IFakeBinding _binding =
    default!;

  private PlayerLogic.Settings _settings = default!;
  private Player _player = default!;

  public PlayerTest(Node testScene) : base(testScene) { }

  [Setup]
  public async Task Setup() {
    _fixture = new(TestScene.GetTree());

    _appRepo = new();
    _gameRepo = new();
    _logic = new();
    _binding = PlayerLogic.CreateFakeBinding();
    _settings = new PlayerLogic.Settings(
      RotationSpeed: 1.0f,
      StoppingSpeed: 1.0f,
      Gravity: -1.0f,
      MoveSpeed: 1.0f,
      Acceleration: 1.0f,
      JumpImpulseForce: 1.0f,
      JumpForce: 1.0f
    );
    _logic.Setup(logic => logic.Bind()).Returns(_binding);

    _player = new() {
      IsTesting = true,
      PlayerLogic = _logic.Object,
      PlayerBinding = _binding,
      Settings = _settings
    };

    _player.FakeDependency(_appRepo.Object);
    _player.FakeDependency(_gameRepo.Object);

    await _fixture.AddToRoot(_player);
  }

  [Cleanup]
  public async Task Cleanup() => await _fixture.Cleanup();

  [Test]
  public void Initializes() {
    _player.Setup();

    _player.Settings.ShouldNotBeNull();
    _player.PlayerLogic.ShouldBeOfType<PlayerLogic>();
    ((IProvide<IPlayerLogic>)_player).Value().ShouldNotBeNull();
  }

  [Test]
  public void OnReady() {
    _player.OnReady();
    _player.IsPhysicsProcessing().ShouldBeTrue();
  }

  [Test]
  public void OnExitTree() {
    _logic.Setup(logic => logic.Stop());

    _player.OnExitTree();

    _logic.VerifyAll();
  }

  [Test]
  public async Task OnPhysicsProcessJumpsOnInput() {
    Input.ActionPress(GameInputs.Jump);

    _player.OnPhysicsProcess(1d);

    _logic.Verify(logic => logic.Input(It.IsAny<PlayerLogic.Input.Jump>()));
  }

  [Test]
  public void OnPhysicsProcess() {
    _logic.Reset();
    _logic.Setup(
      logic => logic.Input(It.IsAny<PlayerLogic.Input.PhysicsTick>())
    );
    _logic.Setup(
      logic => logic.Input(It.IsAny<PlayerLogic.Input.Moved>())
    );

    _player.OnPhysicsProcess(1d);

    _logic.VerifyAll();
  }

  [Test]
  public void ShouldJump() {
    Player.ShouldJump(true, false).ShouldBe(true);
    Player.ShouldJump(true, true).ShouldBe(true);
    Player.ShouldJump(false, false).ShouldBe(false);
  }

  [Test]
  public void GetGlobalInputVector() {
    var cameraBasis = Basis.Identity;
    var expected = Vector3.Zero;

    _player.GetGlobalInputVector(cameraBasis).ShouldBe(expected);
  }

  [Test]
  public void GetNextRotationBasis() {
    var direction = Vector3.Forward;
    var expected = Basis.Identity;

    _player.GetNextRotationBasis(direction, 0d, 0f).ShouldBe(expected);
  }

  [Test]
  public void IsMovingChecks() {
    _player.Velocity = Vector3.Forward * 10;
    _player.IsMovingHorizontally().ShouldBe(true);
    _player.IsStopped().ShouldBe(false);
  }

  [Test]
  public void Pushed() {
    _logic.Reset();
    _logic.Setup(logic => logic.Input(It.IsAny<PlayerLogic.Input.Pushed>()));

    _player.Push(Vector3.Forward);
    _logic.VerifyAll();
  }

  [Test]
  public void CoinCollector() => _player.CenterOfMass.ShouldBeOfType<Vector3>();

  [Test]
  public void Dies() {
    _logic.Reset();
    _logic.Setup(logic => logic.Input(It.IsAny<PlayerLogic.Input.Killed>()));
    _player.Kill();
    _logic.VerifyAll();
  }

  [Test]
  public void ChangesVelocityAfterMovementIsComputed() {
    _player.OnResolved();

    _binding.Output(
      new PlayerLogic.Output.MovementComputed(Basis.Identity, Vector3.Forward)
    );

    _player.Velocity.ShouldBe(Vector3.Forward);
    _player.Transform.Basis.ShouldBe(Basis.Identity);
  }

  [Test]
  public void ChangesVelocityWhenTold() {
    _player.OnResolved();

    _binding.Output(
      new PlayerLogic.Output.VelocityChanged(Vector3.Forward)
    );

    _player.Velocity.ShouldBe(Vector3.Forward);
  }

  [Test]
  public void SaveData() {
    _player.SaveId.ShouldBeOfType<string>();
    _player.GetSaveData().ShouldBeOfType<PlayerData>();
    _player.RestoreSaveData(
      new PlayerData(
        Transform3D.Identity,
        Vector3.Zero,
        new PlayerLogic.State.Idle()
      )
    );
  }
}
