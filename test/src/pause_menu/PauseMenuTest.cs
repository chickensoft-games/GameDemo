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
  private Mock<IButton> _saveButton = default!;
  private Mock<IAnimationPlayer> _animationPlayer = default!;
  private Mock<IAnimationPlayer> _saveOverlayAnimationPlayer = default!;
  private Mock<IVBoxContainer> _saveOverlay = default!;
  private PauseMenu _menu = default!;

  public PauseMenuTest(Node testScene) : base(testScene) { }

  [Setup]
  public void Setup() {
    _mainMenuButton = new Mock<IButton>();
    _resumeButton = new Mock<IButton>();
    _saveButton = new Mock<IButton>();
    _animationPlayer = new Mock<IAnimationPlayer>();
    _saveOverlayAnimationPlayer = new Mock<IAnimationPlayer>();
    _saveOverlay = new Mock<IVBoxContainer>();

    _menu = new PauseMenu {
      MainMenuButton = _mainMenuButton.Object,
      ResumeButton = _resumeButton.Object,
      SaveButton = _saveButton.Object,
      AnimationPlayer = _animationPlayer.Object,
      SaveOverlayAnimationPlayer = _saveOverlayAnimationPlayer.Object,
      SaveOverlay = _saveOverlay.Object
    };
  }

  [Test]
  public void Draw() {
    _menu._Draw();
    _resumeButton.Object.HasFocus().ShouldBeTrue();
  }

  [Test]
  public void Subscribes() {
    _menu.OnReady();
    _mainMenuButton.VerifyAdd(menu => menu.Pressed += _menu.OnMainMenuPressed);
    _resumeButton.VerifyAdd(menu => menu.Pressed += _menu.OnResumePressed);
    _saveButton.VerifyAdd(menu => menu.Pressed += _menu.OnSavePressed);
    _animationPlayer
      .VerifyAdd(menu => menu.AnimationFinished += _menu.OnAnimationFinished);

    _menu.OnExitTree();
    _mainMenuButton
      .VerifyRemove(menu => menu.Pressed -= _menu.OnMainMenuPressed);
    _resumeButton
      .VerifyRemove(menu => menu.Pressed -= _menu.OnResumePressed);
    _saveButton
      .VerifyRemove(menu => menu.Pressed -= _menu.OnSavePressed);
    _animationPlayer
      .VerifyRemove(
        menu => menu.AnimationFinished -= _menu.OnAnimationFinished
      );
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

  [Test]
  public void OnSavePressed() {
    var called = false;
    _menu.Save += () => called = true;

    _menu.OnSavePressed();

    called.ShouldBeTrue();
  }

  [Test]
  public void OnSaveStarted() {
    _saveOverlayAnimationPlayer
      .Setup(player => player.Play("save_fade_in", -1, 1, false));
    _menu.OnSaveStarted();
    _saveOverlayAnimationPlayer.VerifyAll();
  }

  [Test]
  public void OnSaveFinished() {
    _saveOverlayAnimationPlayer
      .Setup(player => player.Play("save_fade_out", -1, 1, false));
    _menu.OnSaveFinished();
    _saveOverlayAnimationPlayer.VerifyAll();
  }
}
