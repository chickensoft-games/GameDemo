namespace GameDemo.Tests;

using System;
using System.Diagnostics.CodeAnalysis;
using Chickensoft.AutoInject;
using Chickensoft.GodotNodeInterfaces;
using Chickensoft.GoDotTest;
using Godot;
using Moq;
using Shouldly;

[
  SuppressMessage(
    "Design",
    "CA1001",
    Justification = "Disposable field is Godot object; Godot will dispose"
  )
]
public class PlayerModelTest : TestClass
{
  private Mock<IPlayerLogic> _playerLogic = default!;
  private PlayerLogic.IFakeBinding _playerBinding = default!;
  private Mock<IAnimationTree> _animationTree = default!;
  private Mock<IAnimationNodeStateMachinePlayback> _animStateMachine = default!;
  private Mock<INode3D> _visualRoot = default!;
  private Mock<INode3D> _centerRoot = default!;
  private Mock<ITimer> _blinkTimer = default!;
  private PlayerModel _model = default!;
  private PlayerLogic.Settings _settings = default!;

  public PlayerModelTest(Node testScene) : base(testScene) { }

  [Setup]
  public void Setup()
  {
    _playerLogic = new Mock<IPlayerLogic>();
    _playerBinding = PlayerLogic.CreateFakeBinding();
    _animationTree = new Mock<IAnimationTree>();
    _animStateMachine = new Mock<IAnimationNodeStateMachinePlayback>();
    _visualRoot = new Mock<INode3D>();
    _centerRoot = new Mock<INode3D>();
    _blinkTimer = new Mock<ITimer>();
    _settings = new PlayerLogic.Settings(
      RotationSpeed: 1.0f,
      StoppingSpeed: 1.0f,
      Gravity: -1.0f,
      MoveSpeed: 1.0f,
      Acceleration: 1.0f,
      JumpImpulseForce: 1.0f,
      JumpForce: 1.0f
    );

    _playerLogic.Setup(logic => logic.Bind()).Returns(_playerBinding);

    _model = new PlayerModel()
    {
      AnimationTree = _animationTree.Object,
      AnimationStateMachine = _animStateMachine.Object
    };

    (_model as IAutoInit).IsTesting = true;

    _model.FakeNodeTree(new()
    {
      ["%AnimationTree"] = _animationTree.Object,
      ["%VisualRoot"] = _visualRoot.Object,
      ["%CenterRoot"] = _centerRoot.Object,
      ["%BlinkTimer"] = _blinkTimer.Object,
    });

    _model.FakeDependency(_playerLogic.Object);
    _model.FakeDependency(_settings);

    _model._Notification((int)Node.NotificationEnterTree);
  }

  [Test]
  public void OnEnterTree()
  {
    _model.OnEnterTree();

    _blinkTimer.VerifyAdd(timer => timer.Timeout += It.IsAny<Action>());
  }

  [Test]
  public void OnExitTree()
  {
    _model.OnResolved();
    _model.OnExitTree();

    _blinkTimer.VerifyRemove(timer => timer.Timeout -= It.IsAny<Action>());
  }

  [Test]
  public void Idles()
  {
    _animStateMachine.Setup(sm => sm.Travel("idle", true));

    _model.OnResolved();
    _playerBinding.Output(new PlayerLogic.Output.Animations.Idle());
    _animStateMachine.VerifyAll();
  }

  [Test]
  public void Runs()
  {
    _animStateMachine.Setup(sm => sm.Travel("run", true));
    _model.OnResolved();
    _playerBinding.Output(new PlayerLogic.Output.Animations.Move());
    _animStateMachine.VerifyAll();
  }

  [Test]
  public void Jumps()
  {
    _animStateMachine.Setup(sm => sm.Travel("jump", true));
    _model.OnResolved();
    _playerBinding.Output(new PlayerLogic.Output.Animations.Jump());
    _animStateMachine.VerifyAll();
  }

  [Test]
  public void Falls()
  {
    _animStateMachine.Setup(sm => sm.Travel("fall", true));
    _model.OnResolved();
    _playerBinding.Output(new PlayerLogic.Output.Animations.Fall());
    _animStateMachine.VerifyAll();
  }

  [Test]
  public void MoveSpeedChanged()
  {
    _model.AnimationStateMachine = _animStateMachine.Object;

    _animationTree.Setup(tree => tree.Set(
      "parameters/main_animations/move/blend_position", 0.5f
    ));
    _model.OnResolved();
    _playerBinding.Output(new PlayerLogic.Output.MoveSpeedChanged(0.5f));
    _animationTree.VerifyAll();
  }

  [Test]
  public void MovementComputedAffectsLeanWhenGrounded()
  {
    _model.OnResolved();
    _playerBinding.Output(
      new PlayerLogic.Output.MovementComputed(
        Basis.Identity, Vector3.Forward, Vector2.Up, 1d
      )
    );

    _playerLogic
      .Setup(logic => logic.Value)
      .Returns(new PlayerLogic.State.Idle());

    _animationTree.Verify(
      tree => tree.Set(PlayerModel.LEAN_ADD, It.IsAny<Variant>())
    );

    _animationTree.Verify(
      tree => tree.Set(PlayerModel.LEAN_DIRECTION_BLEND, It.IsAny<Variant>())
    );
  }

  [Test]
  public void GetTarget()
  {
    PlayerModel.GetTarget(1, new PlayerLogic.State.Jumping()).ShouldBe(0);
    PlayerModel.GetTarget(1, new PlayerLogic.State.Idle()).ShouldBe(1);
  }

  [Test]
  public void Blinks()
  {
    _model.OnBlink();

    _animationTree.Verify(tree => tree.Set(PlayerModel.BLINK_REQUEST, (int)AnimationNodeOneShot.OneShotRequest.Fire));
  }

  [Test]
  public void Ready()
  {
    var sm = new AnimationNodeStateMachinePlayback();
    _animationTree
      .Setup(tree => tree.Get(PlayerModel.ANIM_STATE_MACHINE))
      .Returns(sm);

    _model.OnReady();

    _model.AnimationStateMachine
      .GetTargetObj<AnimationNodeStateMachinePlayback>().ShouldBe(sm);
  }
}
