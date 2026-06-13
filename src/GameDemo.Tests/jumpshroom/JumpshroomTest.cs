namespace GameDemo.Tests;

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
[Collection(Constants.Headless)]
public partial class JumpshroomTest
{
  public partial class FakePushEnabled : Node3D, IPushEnabled
  {
    public void Push(Vector3 force) { }
  }

  private readonly LogicBlock.FakeBinding _binding = LogicBlock.CreateFakeBinding();

  private readonly Mock<IJumpshroomLogic> _logic = new();
  private readonly Mock<IGameRepo> _gameRepo = new();
  private readonly Mock<IAnimationPlayer> _animPlayer = new();
  private readonly Mock<IArea3D> _area3D = new();
  private readonly Mock<ITimer> _cooldownTimer = new();
  private readonly Jumpshroom _shroom;

  public JumpshroomTest()
  {
    _logic.Setup(logic => logic.Bind()).Returns(_binding);

    _shroom = new Jumpshroom
    {
      JumpshroomLogic = _logic.Object,
      JumpshroomBinding = _binding,
      AnimationPlayer = _animPlayer.Object,
      Area3D = _area3D.Object,
      CooldownTimer = _cooldownTimer.Object
    };

    (_shroom as IAutoInit).IsTesting = true;

    _shroom.FakeDependency(_gameRepo.Object);

    _shroom._Notification(-1);
  }

  [Fact]
  public void InitializesValues()
  {
    _shroom.Setup();

    _shroom.JumpshroomLogic.ShouldNotBeNull();
  }

  [Fact]
  public void Subscribes()
  {
    _shroom.OnResolved();

    _animPlayer.VerifyAdd(
      player => player.AnimationFinished += _shroom.OnAnimationFinished
    );
    _area3D.VerifyAdd(
      area => area.BodyEntered += _shroom.OnAreaBodyEntered
    );
    _cooldownTimer.VerifyAdd(
      timer => timer.Timeout += _shroom.OnCooldownTimeout
    );

    _shroom.OnExitTree();

    _animPlayer.VerifyRemove(
      player => player.AnimationFinished -= _shroom.OnAnimationFinished
    );
    _area3D.VerifyRemove(
      area => area.BodyEntered -= _shroom.OnAreaBodyEntered
    );
    _cooldownTimer.VerifyRemove(
      timer => timer.Timeout -= _shroom.OnCooldownTimeout
    );
  }

  [Fact]
  public void ShroomLoadedLaunches()
  {
    _logic.Reset();
    _logic.Setup(
      logic => logic.Input(It.IsAny<JumpshroomLogicState.Input.Launch>())
    );
    _shroom.OnShroomLoaded();
    _logic.VerifyAll();
  }

  [Fact]
  public void OnAnimationFinishedCompletesLaunch()
  {
    _logic.Reset();
    _logic.Setup(
      logic => logic.Input(It.IsAny<JumpshroomLogicState.Input.LaunchCompleted>())
    );
    _shroom.OnAnimationFinished("launch");
    _logic.VerifyAll();
  }

  [Fact]
  public void OnAreaBodyEnteredHitsIfPushEnabled()
  {
    _logic.Reset();
    var target = new FakePushEnabled();
    _logic.Setup(
      logic => logic.Input(in It.Ref<JumpshroomLogicState.Input.Hit>.IsAny)
    );
    _shroom.OnAreaBodyEntered(target);

    _logic.VerifyAll();
  }

  [Fact]
  public void OnCooldownTimeoutCompletesCooldown()
  {
    _logic.Reset();
    _logic.Setup(
      logic => logic.Input(
        in It.Ref<JumpshroomLogicState.Input.CooldownCompleted>.IsAny
      )
    );
    _shroom.OnCooldownTimeout();
    _logic.VerifyAll();
  }

  [Fact]
  public void AnimatesBounce()
  {
    _animPlayer.Setup(player => player.Play("bounce", -1, 1, false));

    _shroom.OnResolved();
    _binding.Output(new JumpshroomLogicState.Output.Animate());

    _animPlayer.VerifyAll();
  }

  [Fact]
  public void StartsCooldownTimer()
  {
    _cooldownTimer.Setup(timer => timer.Start(-1));

    _shroom.OnResolved();
    _binding.Output(new JumpshroomLogicState.Output.StartCooldownTimer());

    _cooldownTimer.VerifyAll();
  }
}
