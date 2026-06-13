namespace GameDemo.Tests;

using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Chickensoft.AutoInject;
using Chickensoft.Collections;
using Chickensoft.GodotNodeInterfaces;
using Chickensoft.LogicBlocks;
using Godot;
using Moq;
using Shouldly;

[
  SuppressMessage(
    "Design",
    "CA1001",
    Justification = "Disposable field is a Godot object; Godot will dispose"
  )
]
[Collection("GodotHeadless")]
public partial class CoinTest
{
  private readonly GodotHeadlessFixture _godot;

  public partial class FakeCoinCollector : Node3D, ICoinCollector
  {
    public Vector3 CenterOfMass => Vector3.Zero;
    public void Collect(ICoin coin) { }
  }

  private readonly Mock<IGameRepo> _gameRepo = new();
  private readonly Mock<IAnimationPlayer> _animPlayer = new();
  private readonly Mock<INode3D> _coinModel = new();
  private readonly Mock<ICoinLogic> _logic = new();
  private readonly EntityTable _entityTable = new();

  private readonly LogicBlock.FakeBinding _binding = LogicBlock.CreateFakeBinding();

  private readonly Coin _coin;

  public CoinTest(GodotHeadlessFixture godot)
  {
    _godot = godot;

    _logic.Setup(logic => logic.Bind()).Returns(_binding);

    _coin = new Coin
    {
      AnimationPlayer = _animPlayer.Object,
      CoinModel = _coinModel.Object,
      CoinLogic = _logic.Object,
      CoinBinding = _binding
    };

    (_coin as IAutoInit).IsTesting = true;

    _coin.FakeDependency(_gameRepo.Object);
    _coin.FakeDependency(_entityTable);

    // For tests that run in the actual node tree.
    _coin.FakeNodeTree(new()
    {
      ["%AnimationPlayer"] = _animPlayer.Object,
      ["%CoinModel"] = _coinModel.Object
    });
  }

  [Fact]
  public void Initializes()
  {
    _coin.Setup();

    _coin.Settings.ShouldNotBeNull();
    _coin.CoinLogic.ShouldNotBeNull();
  }

  [Fact]
  public void OnPhysicsProcess()
  {
    _logic.Reset();
    _logic.Setup(
      logic => logic.Input(in It.Ref<CoinLogicState.Input.PhysicsProcess>.IsAny)
    );

    _coin.OnPhysicsProcess(1f);

    _logic.VerifyAll();
  }

  [Fact]
  public void OnCollectorDetectorBodyEntered()
  {
    _logic.Reset();
    _logic.Setup(
      logic => logic.Input(in It.Ref<CoinLogicState.Input.StartCollection>.IsAny)
    );
    var collector = new FakeCoinCollector();

    _coin.OnCollectorDetectorBodyEntered(collector);

    _logic.VerifyAll();
  }

  [Fact]
  public void StartsCollectionProcess()
  {
    _coin.OnResolved();
    var state = new Mock<CoinLogicState.Collecting>();
    _animPlayer.Setup(player => player.Play("collect", -1, 1, false));

    _binding.SetState(state.Object);

    _coin.IsPhysicsProcessing().ShouldBeTrue();
    _animPlayer.VerifyAll();
  }

  [Fact]
  public void InitializesDetectorAreaDynamically()
  {
    // This test has to be run in the actual scene tree since it verifies the
    // coin loads and adds a real scene.
    var tree = _godot.Tree;
    tree.Root.AddChild(_coin);

    _coin.GetChildren().ShouldContain(child => child is Area3D);

    _coin.QueueFree();
  }

  [Fact]
  public void MovesAndSelfDestructs()
  {
    // This test has to be run in the actual scene tree since it verifies the
    // coin changes its GlobalPosition.
    var tree = _godot.Tree;
    tree.Root.AddChild(_coin);

    _binding.Output(new CoinLogicState.Output.Move(Vector3.One));

    _coin.GlobalPosition.ShouldBe(Vector3.One);

    _binding.Output(new CoinLogicState.Output.SelfDestruct());

    _coin.IsQueuedForDeletion().ShouldBeTrue();

    _coin.QueueFree();
  }
}
