namespace GameDemo.Tests;

using System.Threading.Tasks;
using Chickensoft.AutoInject;
using Chickensoft.GodotNodeInterfaces;
using Chickensoft.GoDotTest;
using Chickensoft.GodotTestDriver;
using Chickensoft.GodotTestDriver.Util;
using Godot;
using Moq;
using Shouldly;

public class PlayerModelTest : TestClass {
  private Mock<IPlayerLogic> _playerLogic = default!;
  private PlayerLogic.IFakeBinding _playerBinding = default!;
  private Mock<IAnimationTree> _animationTree = default!;
  private Mock<IAnimationNodeStateMachinePlayback> _animStateMachine = default!;
  private PlayerModel _model = default!;

  public PlayerModelTest(Node testScene) : base(testScene) { }

  [Setup]
  public void Setup() {
    _playerLogic = new Mock<IPlayerLogic>();
    _playerBinding = PlayerLogic.CreateFakeBinding();
    _animationTree = new Mock<IAnimationTree>();
    _animStateMachine = new Mock<IAnimationNodeStateMachinePlayback>();

    _playerLogic.Setup(logic => logic.Bind()).Returns(_playerBinding);

    _model = new PlayerModel() {
      AnimationTree = _animationTree.Object,
      AnimationStateMachine = _animStateMachine.Object
    };

    _model.FakeNodeTree(new() {
      ["%AnimationTree"] = _animationTree.Object
    });

    _model.FakeDependency(_playerLogic.Object);
  }

  [Test]
  public async Task ReadyGetsAnimationStateMachinePlayback() {
    // This test has to be conducted inside the scene tree so that we can
    // ensure it loads the actual animation state machine playback
    var tree = TestScene.GetTree();
    var fixture = new Fixture(tree);

    _animationTree.Setup(tree => tree.Get(
      "parameters/main_animations/playback"
    )).Returns(Variant.From(new AnimationNodeStateMachinePlayback()));

    await fixture.AddToRoot(_model);

    await tree.WaitForEvents();
    _model.OnReady();

    _model.AnimationStateMachine.ShouldNotBeNull();

    await fixture.Cleanup();
  }

  [Test]
  public void Idles() {
    _animStateMachine.Setup(sm => sm.Travel("idle", true));
    _model.OnResolved();
    _playerBinding.Output(new PlayerLogic.Output.Animations.Idle());
    _animStateMachine.VerifyAll();
  }

  [Test]
  public void Moves() {
    _animStateMachine.Setup(sm => sm.Travel("move", true));
    _model.OnResolved();
    _playerBinding.Output(new PlayerLogic.Output.Animations.Move());
    _animStateMachine.VerifyAll();
  }

  [Test]
  public void Jumps() {
    _animStateMachine.Setup(sm => sm.Travel("jump", true));
    _model.OnResolved();
    _playerBinding.Output(new PlayerLogic.Output.Animations.Jump());
    _animStateMachine.VerifyAll();
  }

  [Test]
  public void Falls() {
    _animStateMachine.Setup(sm => sm.Travel("fall", true));
    _model.OnResolved();
    _playerBinding.Output(new PlayerLogic.Output.Animations.Fall());
    _animStateMachine.VerifyAll();
  }

  [Test]
  public void MoveSpeedChanged() {
    _animationTree.Setup(tree => tree.Set(
      "parameters/main_animations/move/blend_position", 0.5f
    ));
    _model.OnResolved();
    _playerBinding.Output(new PlayerLogic.Output.MoveSpeedChanged(0.5f));
    _animationTree.VerifyAll();
  }
}
