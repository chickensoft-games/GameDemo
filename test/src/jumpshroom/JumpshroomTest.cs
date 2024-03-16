namespace GameDemo.Tests;

using Chickensoft.GodotNodeInterfaces;
using Chickensoft.GoDotTest;
using Godot;
using Moq;
using Shouldly;

public partial class JumpshroomTest : TestClass {
  public partial class FakePushEnabled : Node3D, IPushEnabled {
    public void Push(Vector3 force) { }
  }

  private JumpshroomLogic.IFakeBinding _binding = default!;

  private Mock<IJumpshroomLogic> _logic = default!;
  private Mock<IGameRepo> _gameRepo = default!;
  private Mock<IAnimationPlayer> _animPlayer = default!;
  private Mock<IArea3D> _area3D = default!;
  private Mock<ITimer> _cooldownTimer = default!;
  private Jumpshroom _shroom = default!;

  public JumpshroomTest(Node testScene) : base(testScene) { }

  [Setup]
  public void Setup() {
    _binding = JumpshroomLogic.CreateFakeBinding();

    _logic = new();
    _gameRepo = new();
    _animPlayer = new();
    _area3D = new();
    _cooldownTimer = new();

    _logic.Setup(logic => logic.Bind()).Returns(_binding);

    _shroom = new Jumpshroom {
      IsTesting = true,
      JumpshroomLogic = _logic.Object,
      JumpshroomBinding = _binding,
      AnimationPlayer = _animPlayer.Object,
      Area3D = _area3D.Object,
      CooldownTimer = _cooldownTimer.Object
    };

    _shroom.FakeDependency(_gameRepo.Object);
  }

  [Test]
  public void InitializesValues() {
    _shroom.Setup();

    _shroom.JumpshroomLogic.ShouldNotBeNull();
  }

  [Test]
  public void Subscribes() {
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

  [Test]
  public void ShroomLoadedLaunches() {
    _logic.Reset();
    _logic.Setup(
      logic => logic.Input(It.IsAny<JumpshroomLogic.Input.Launch>())
    );
    _shroom.OnShroomLoaded();
    _logic.VerifyAll();
  }

  [Test]
  public void OnAnimationFinishedCompletesLaunch() {
    _logic.Reset();
    _logic.Setup(
      logic => logic.Input(It.IsAny<JumpshroomLogic.Input.LaunchCompleted>())
    );
    _shroom.OnAnimationFinished("launch");
    _logic.VerifyAll();
  }

  [Test]
  public void OnAreaBodyEnteredHitsIfPushEnabled() {
    _logic.Reset();
    var target = new FakePushEnabled();
    _logic.Setup(
      logic => logic.Input(in It.Ref<JumpshroomLogic.Input.Hit>.IsAny)
    );
    _shroom.OnAreaBodyEntered(target);

    _logic.VerifyAll();
  }

  [Test]
  public void OnCooldownTimeoutCompletesCooldown() {
    _logic.Reset();
    _logic.Setup(
      logic => logic.Input(
        in It.Ref<JumpshroomLogic.Input.CooldownCompleted>.IsAny
      )
    );
    _shroom.OnCooldownTimeout();
    _logic.VerifyAll();
  }

  [Test]
  public void AnimatesBounce() {
    _animPlayer.Setup(player => player.Play("bounce", -1, 1, false));

    _shroom.OnResolved();
    _binding.Output(new JumpshroomLogic.Output.Animate());

    _animPlayer.VerifyAll();
  }

  [Test]
  public void StartsCooldownTimer() {
    _cooldownTimer.Setup(timer => timer.Start(-1));

    _shroom.OnResolved();
    _binding.Output(new JumpshroomLogic.Output.StartCooldownTimer());

    _cooldownTimer.VerifyAll();
  }
}
