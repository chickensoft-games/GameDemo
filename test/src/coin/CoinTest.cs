namespace GameDemo.Tests;

using System;
using System.Threading.Tasks;
using Chickensoft.GodotNodeInterfaces;
using Chickensoft.GoDotTest;
using Chickensoft.LogicBlocks;
using Godot;
using GodotTestDriver;
using Moq;
using Shouldly;

public partial class CoinTest : TestClass {
  public partial class FakeCoinCollector : Node3D, ICoinCollector {
    public Vector3 CenterOfMass => Vector3.Zero;
    public void Collect(ICoin coin) { }
  }

  private Mock<IGameRepo> _gameRepo = default!;
  private Mock<IAnimationPlayer> _animPlayer = default!;
  private Mock<INode3D> _coinModel = default!;
  private Mock<ICoinLogic> _logic = default!;

  private Logic<CoinLogic.IState, Func<object, CoinLogic.IState>, CoinLogic.IState,
    Action<CoinLogic.IState?>>.IFakeBinding _binding = default!;

  private Coin _coin = default!;

  public CoinTest(Node testScene) : base(testScene) { }

  [Setup]
  public void Setup() {
    _gameRepo = new();
    _animPlayer = new();
    _coinModel = new();
    _logic = new();
    _binding = CoinLogic.CreateFakeBinding();

    _logic.Setup(logic => logic.Bind()).Returns(_binding);

    _coin = new Coin {
      IsTesting = true,
      AnimationPlayer = _animPlayer.Object,
      CoinModel = _coinModel.Object,
      CoinLogic = _logic.Object,
      CoinBinding = _binding
    };

    _coin.FakeDependency(_gameRepo.Object);

    // For tests that run in the actual node tree.
    _coin.FakeNodeTree(new() {
      ["%AnimationPlayer"] = _animPlayer.Object, ["%CoinModel"] = _coinModel.Object
    });
  }

  [Test]
  public void Initializes() {
    _coin.Setup();

    _coin.Settings.ShouldNotBeNull();
    _coin.CoinLogic.ShouldNotBeNull();
  }

  [Test]
  public void OnPhysicsProcess() {
    _logic.Reset();
    _logic.Setup(
      logic => logic.Input(It.IsAny<CoinLogic.Input.PhysicsProcess>())
    );

    _coin.OnPhysicsProcess(1f);

    _logic.VerifyAll();
  }

  [Test]
  public void OnCollectorDetectorBodyEntered() {
    _logic.Reset();
    _logic.Setup(
      logic => logic.Input(It.IsAny<CoinLogic.Input.StartCollection>())
    );
    var collector = new FakeCoinCollector();

    _coin.OnCollectorDetectorBodyEntered(collector);

    _logic.VerifyAll();
  }

  [Test]
  public void StartsCollectionProcess() {
    _coin.OnResolved();
    var state = new Mock<CoinLogic.State.ICollecting>();
    _animPlayer.Setup(player => player.Play("collect", -1, 1, false));

    _binding.SetState(state.Object);

    _coin.IsPhysicsProcessing().ShouldBeTrue();
    _animPlayer.VerifyAll();
  }

  [Test]
  public async Task InitializesDetectorAreaDynamically() {
    // This test has to be run in the actual scene tree since it verifies the
    // coin loads and adds a real scene.
    var tree = TestScene.GetTree();
    var fixture = new Fixture(tree);
    await fixture.AddToRoot(_coin);

    _coin.GetChildren().ShouldContain(child => child is Area3D);

    await fixture.Cleanup();
  }

  [Test]
  public async Task MovesAndSelfDestructs() {
    // This test has to be run in the actual scene tree since it verifies the
    // coin changes its GlobalPosition.
    var tree = TestScene.GetTree();
    var fixture = new Fixture(tree);
    await fixture.AddToRoot(_coin);

    _binding.Output(new CoinLogic.Output.Move(Vector3.One));

    _coin.GlobalPosition.ShouldBe(Vector3.One);

    _binding.Output(new CoinLogic.Output.SelfDestruct());

    _coin.IsQueuedForDeletion().ShouldBeTrue();

    await fixture.Cleanup();
  }

  [Test]
  public void SaveData() {
    _coin.SaveId.ShouldBeOfType<string>();
    _coin.GetSaveData().ShouldBeOfType<CoinData>();
    _coin.RestoreSaveData(
      new CoinData("name", Transform3D.Identity, new CoinLogic.State.Idle())
    );
  }
}
