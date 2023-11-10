namespace GameDemo.Tests;

using System.Threading.Tasks;
using Chickensoft.GodotNodeInterfaces;
using Chickensoft.GoDotTest;
using Godot;
using Moq;
using Shouldly;

public class PauseMenuTest : TestClass {
  private Mock<IButton> _mainMenuButton = default!;
  private Mock<IButton> _resumeButton = default!;
  private Mock<IAnimationPlayer> _animationPlayer = default!;
  private PauseMenu _menu = default!;

  public PauseMenuTest(Node testScene) : base(testScene) { }

  [Setup]
  public void Setup() {
    _mainMenuButton = new Mock<IButton>();
    _resumeButton = new Mock<IButton>();
    _animationPlayer = new Mock<IAnimationPlayer>();

    _menu = new PauseMenu {
      MainMenuButton = _mainMenuButton.Object,
      ResumeButton = _resumeButton.Object,
      AnimationPlayer = _animationPlayer.Object
    };
  }

  [Test]
  public void Subscribes() {
    _menu.OnReady();
    _mainMenuButton.VerifyAdd(menu => menu.Pressed += _menu.OnMainMenuPressed);

    _menu.OnExitTree();
    _mainMenuButton
      .VerifyRemove(menu => menu.Pressed -= _menu.OnMainMenuPressed);
  }

  [Test]
  public async Task SignalsMainMenuButtonPressed() {
    var signal = _menu.ToSignal(_menu, PauseMenu.SignalName.MainMenu);

    _menu.OnMainMenuPressed();

    await signal;

    signal.IsCompleted.ShouldBeTrue();
  }

  [Test]
  public async Task SignalsResumeButtonPressed() {
    var signal = _menu.ToSignal(_menu, PauseMenu.SignalName.Resume);

    _menu.OnResumePressed();

    await signal;

    signal.IsCompleted.ShouldBeTrue();
  }

  [Test]
  public async Task SignalsTransitionCompleted() {
    var signal =
      _menu.ToSignal(_menu, PauseMenu.SignalName.TransitionCompleted);

    _menu.OnAnimationFinished("fade_in");

    await signal;

    signal.IsCompleted.ShouldBeTrue();
  }

  [Test]
  public void FadesIn() {
    _animationPlayer.Setup(player => player.Play("fade_in", -1, 1, false));
    _menu.FadeIn();
    _animationPlayer.VerifyAll();
  }

  [Test]
  public void FadesOut() {
    _animationPlayer.Setup(player => player.Play("fade_out", -1, 1, false));
    _menu.FadeOut();
    _animationPlayer.VerifyAll();
  }
}
