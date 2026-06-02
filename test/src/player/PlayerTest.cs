namespace GameDemo.Tests;

using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Chickensoft.AutoInject;
using Chickensoft.Collections;
using Chickensoft.GoDotTest;
using Chickensoft.GodotTestDriver;
using Chickensoft.LogicBlocks;
using Chickensoft.SaveFileBuilder;
using Godot;
using Moq;
using Shouldly;

[
  SuppressMessage(
    "Design",
    "CA1001",
    Justification = "Disposable field is added to TestDriver fixture"
  )
]
public class PlayerTest : TestClass
{
  private Fixture _fixture = default!;
  private IAppRepo _appRepo = default!;
  private IGameRepo _gameRepo = default!;
  private Mock<IPlayerLogic> _logic = default!;
  private EntityTable _entityTable = default!;
  private Mock<ISaveChunk<GameData>> _gameChunk = default!;

  private LogicBlock.FakeBinding _binding = default!;

  private PlayerLogic.Settings _settings = default!;
  private Player _player = default!;

  public PlayerTest(Node testScene) : base(testScene) { }

  [Setup]
  public async Task Setup()
  {
    _fixture = new(TestScene.GetTree());

    _appRepo = new AppRepo();
    _gameRepo = new GameRepo();
    _logic = new();
    _binding = LogicBlock.CreateFakeBinding();
    _settings = new PlayerLogic.Settings(
      RotationSpeed: 1.0f,
      StoppingSpeed: 1.0f,
      Gravity: -1.0f,
      MoveSpeed: 1.0f,
      Acceleration: 1.0f,
      JumpImpulseForce: 1.0f,
      JumpForce: 1.0f
    );
    _entityTable = new();
    _gameChunk = new();
    _logic.Setup(logic => logic.Bind()).Returns(_binding);

    _player = new()
    {
      RotationSpeed = _settings.RotationSpeed,
      StoppingSpeed = _settings.StoppingSpeed,
      Gravity = _settings.Gravity,
      MoveSpeed = _settings.MoveSpeed,
      Acceleration = _settings.Acceleration,
      JumpImpulseForce = _settings.JumpImpulseForce,
      JumpForce = _settings.JumpForce,
      PlayerLogic = _logic.Object,
      PlayerBinding = _binding,
      Settings = _settings
    };

    (_player as IAutoInit).IsTesting = true;

    _player.FakeDependency(_appRepo);
    _player.FakeDependency(_gameRepo);
    _player.FakeDependency(_entityTable);
    _player.FakeDependency(_gameChunk.Object);

    await _fixture.AddToRoot(_player);
  }

  [Cleanup]
  public async Task Cleanup() => await _fixture.Cleanup();

  [Test]
  public void Initializes()
  {
    _player.Setup();

    _player.Settings.ShouldNotBeNull();
    _player.PlayerLogic.ShouldBeOfType<PlayerLogic>();
    ((IProvide<IPlayerLogic>)_player).Value().ShouldNotBeNull();
    ((IProvide<PlayerLogic.Settings>)_player).Value().ShouldBe(_settings);
  }

  [Test]
  public void OnReady()
  {
    _player.OnReady();
    _player.IsPhysicsProcessing().ShouldBeTrue();
  }

  [Test]
  public void OnExitTree()
  {
    _logic.Setup(logic => logic.Stop());

    _player.OnExitTree();

    _logic.VerifyAll();
  }

  [Test]
  public void OnPhysicsProcessJumpsOnInput()
  {
    Input.ActionPress(GameInputs.Jump);

    _player.OnPhysicsProcess(1d);

    _logic.Verify(
      logic => logic.Input(in It.Ref<PlayerLogicState.Input.Jump>.IsAny)
    );
  }

  [Test]
  public void OnPhysicsProcess()
  {
    _logic.Reset();
    _logic.Setup(
      logic => logic.Input(in It.Ref<PlayerLogicState.Input.PhysicsTick>.IsAny)
    );
    _logic.Setup(
      logic => logic.Input(in It.Ref<PlayerLogicState.Input.Moved>.IsAny)
    );

    _player.OnPhysicsProcess(1d);

    _logic.VerifyAll();
  }

  [Test]
  public void ShouldJump()
  {
    Player.ShouldJump(true, false).ShouldBe(true);
    Player.ShouldJump(true, true).ShouldBe(true);
    Player.ShouldJump(false, false).ShouldBe(false);
  }

  [Test]
  public void GetGlobalInputVector()
  {
    var cameraBasis = Basis.Identity;
    var expected = Vector3.Zero;

    _player.GetGlobalInputVector(cameraBasis).ShouldBe(expected);
  }

  [Test]
  public void GetNextRotationBasis()
  {
    var direction = Vector3.Forward;
    var expected = Basis.Identity;

    _player.GetNextRotationBasis(direction, 0d, 0f).ShouldBe(expected);
  }

  [Test]
  public void IsMovingChecks()
  {
    _player.Velocity = Vector3.Forward * 10;
    _player.IsMovingHorizontally().ShouldBe(true);
  }

  [Test]
  public void Pushed()
  {
    _logic.Reset();
    _logic.Setup(
      logic => logic.Input(in It.Ref<PlayerLogicState.Input.Pushed>.IsAny)
    );

    _player.Push(Vector3.Forward);
    _logic.VerifyAll();
  }

  [Test]
  public void CoinCollector() => _player.CenterOfMass.ShouldBeOfType<Vector3>();

  [Test]
  public void Dies()
  {
    _logic.Reset();
    _logic.Setup(
      logic => logic.Input(in It.Ref<PlayerLogicState.Input.Killed>.IsAny)
    );
    _player.Kill();
    _logic.VerifyAll();
  }

  [Test]
  public void ChangesVelocityAfterMovementIsComputed()
  {
    _player.OnResolved();

    _binding.Output(
      new PlayerLogicState.Output.MovementComputed(
        Basis.Identity, Vector3.Forward, Vector2.Zero, 0d
      )
    );

    _player.Velocity.ShouldBe(Vector3.Forward);
    _player.Transform.Basis.ShouldBe(Basis.Identity);
  }

  [Test]
  public void ChangesVelocityWhenTold()
  {
    _player.OnResolved();

    _binding.Output(
      new PlayerLogicState.Output.VelocityChanged(Vector3.Forward)
    );

    _player.Velocity.ShouldBe(Vector3.Forward);
  }

  [Test]
  public void Saves()
  {
    _player.Setup();
    _player.PlayerLogic.Start<PlayerLogicState.Disabled>();

    var chunk = new Mock<ISaveChunk<PlayerData>>();

    var data = _player.PlayerChunk.OnSave(chunk.Object);

    data.GlobalTransform.ShouldBe(_player.GlobalTransform);
    data.StateMachine.Data.ShouldBe(_player.PlayerLogic.Save().Data);
    data.Velocity.ShouldBe(_player.Velocity);
  }

  [Test]
  public void Loads()
  {
    _player.Setup();

    var chunk = new Mock<ISaveChunk<PlayerData>>();

    var logic = new PlayerLogic();
    logic.Set(_appRepo);
    logic.Start<PlayerLogicState.Disabled>();

    _player.PlayerLogic = logic;

    var data = new PlayerData
    {
      GlobalTransform = Transform3D.Identity,
      StateMachine = logic.Save(),
      Velocity = Vector3.Forward
    };

    _player.PlayerChunk.OnLoad(chunk.Object, data);

    _player.GlobalTransform.ShouldBe(Transform3D.Identity);
    _player.Velocity.ShouldBe(Vector3.Forward);
    _player.PlayerLogic.ShouldBeOfType<PlayerLogic>();
  }
}
