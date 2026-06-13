namespace GameDemo.Tests;

using System;
using System.Diagnostics.CodeAnalysis;
using Chickensoft.AutoInject;
using Chickensoft.GodotNodeInterfaces;
using Chickensoft.LogicBlocks;
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
[Collection(Constants.HEADLESS)]
public class PlayerModelTest
{
  private readonly Mock<IPlayerLogic> _playerLogic = new();
  private readonly LogicBlock.FakeBinding _playerBinding = LogicBlock.CreateFakeBinding();
  private readonly Mock<IAnimationTree> _animationTree = new();
  private readonly Mock<IAnimationNodeStateMachinePlayback> _animStateMachine = new();
  private readonly Mock<INode3D> _visualRoot = new();
  private readonly Mock<INode3D> _centerRoot = new();
  private readonly Mock<ITimer> _blinkTimer = new();
  private readonly PlayerModel _model;
  private readonly PlayerLogic.Settings _settings = new(
    RotationSpeed: 1.0f,
    StoppingSpeed: 1.0f,
    Gravity: -1.0f,
    MoveSpeed: 1.0f,
    Acceleration: 1.0f,
    JumpImpulseForce: 1.0f,
    JumpForce: 1.0f
  );

  public PlayerModelTest()
  {
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

  [Fact]
  public void OnEnterTree()
  {
    _model.OnEnterTree();

    _blinkTimer.VerifyAdd(timer => timer.Timeout += It.IsAny<Action>());
  }

  [Fact]
  public void OnExitTree()
  {
    _model.OnResolved();
    _model.OnExitTree();

    _blinkTimer.VerifyRemove(timer => timer.Timeout -= It.IsAny<Action>());
  }

  [Fact]
  public void Idles()
  {
    _animStateMachine.Setup(sm => sm.Travel("idle", true));

    _model.OnResolved();
    _playerBinding.Output(new PlayerLogicState.Output.Animations.Idle());
    _animStateMachine.VerifyAll();
  }

  [Fact]
  public void Runs()
  {
    _animStateMachine.Setup(sm => sm.Travel("run", true));
    _model.OnResolved();
    _playerBinding.Output(new PlayerLogicState.Output.Animations.Move());
    _animStateMachine.VerifyAll();
  }

  [Fact]
  public void Jumps()
  {
    _animStateMachine.Setup(sm => sm.Travel("jump", true));
    _model.OnResolved();
    _playerBinding.Output(new PlayerLogicState.Output.Animations.Jump());
    _animStateMachine.VerifyAll();
  }

  [Fact]
  public void Falls()
  {
    _animStateMachine.Setup(sm => sm.Travel("fall", true));
    _model.OnResolved();
    _playerBinding.Output(new PlayerLogicState.Output.Animations.Fall());
    _animStateMachine.VerifyAll();
  }

  [Fact]
  public void MoveSpeedChanged()
  {
    _model.AnimationStateMachine = _animStateMachine.Object;

    _animationTree.Setup(tree => tree.Set(
      "parameters/main_animations/move/blend_position", 0.5f
    ));
    _model.OnResolved();
    _playerBinding.Output(new PlayerLogicState.Output.MoveSpeedChanged(0.5f));
    _animationTree.VerifyAll();
  }

  [Fact]
  public void MovementComputedAffectsLeanWhenGrounded()
  {
    _model.OnResolved();
    _playerBinding.Output(
      new PlayerLogicState.Output.MovementComputed(
        Basis.Identity, Vector3.Forward, Vector2.Up, 1d
      )
    );

    _playerLogic
      .Setup(logic => logic.State)
      .Returns(new PlayerLogicState.Idle());

    _animationTree.Verify(
      tree => tree.Set(PlayerModel.LEAN_ADD, It.IsAny<Variant>())
    );

    _animationTree.Verify(
      tree => tree.Set(PlayerModel.LEAN_DIRECTION_BLEND, It.IsAny<Variant>())
    );
  }

  [Fact]
  public void GetTarget()
  {
    PlayerModel.GetTarget(1, new PlayerLogicState.Jumping()).ShouldBe(0);
    PlayerModel.GetTarget(1, new PlayerLogicState.Idle()).ShouldBe(1);
  }

  [Fact]
  public void Blinks()
  {
    _model.OnBlink();

    _animationTree.Verify(tree => tree.Set(PlayerModel.BLINK_REQUEST, (int)AnimationNodeOneShot.OneShotRequest.Fire));
  }

  [Fact]
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
