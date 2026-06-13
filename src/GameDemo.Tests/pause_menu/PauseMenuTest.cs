namespace GameDemo.Tests;

using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Chickensoft.GodotNodeInterfaces;
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
public class PauseMenuTest
{
  private readonly GodotHeadlessFixture _godot;
  private readonly Mock<IButton> _mainMenuButton = new();
  private readonly Mock<IButton> _resumeButton = new();
  private readonly Mock<IButton> _saveButton = new();
  private readonly Mock<IAnimationPlayer> _animationPlayer = new();
  private readonly Mock<IAnimationPlayer> _saveOverlayAnimationPlayer = new();
  private readonly Mock<IVBoxContainer> _saveOverlay = new();
  private readonly PauseMenu _menu;

  public PauseMenuTest(GodotHeadlessFixture godot)
  {
    _godot = godot;

    _menu = new PauseMenu
    {
      MainMenuButton = _mainMenuButton.Object,
      ResumeButton = _resumeButton.Object,
      SaveButton = _saveButton.Object,
      AnimationPlayer = _animationPlayer.Object,
      SaveOverlayAnimationPlayer = _saveOverlayAnimationPlayer.Object,
      SaveOverlay = _saveOverlay.Object
    };

    _menu._Notification(-1);
  }

  [Fact]
  public void Subscribes()
  {
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

  [Fact]
  public async Task SignalsMainMenuButtonPressed()
  {
    var signal = _menu.ToSignal(_menu, PauseMenu.SignalName.MainMenu);

    _menu.OnMainMenuPressed();

    await signal;

    signal.IsCompleted.ShouldBeTrue();
  }

  [Fact]
  public async Task SignalsResumeButtonPressed()
  {
    var signal = _menu.ToSignal(_menu, PauseMenu.SignalName.Resume);

    _menu.OnResumePressed();

    await signal;

    signal.IsCompleted.ShouldBeTrue();
  }

  [Fact]
  public async Task SignalsTransitionCompleted()
  {
    var signal =
      _menu.ToSignal(_menu, PauseMenu.SignalName.TransitionCompleted);

    _menu.OnAnimationFinished("fade_in");

    await signal;

    signal.IsCompleted.ShouldBeTrue();
  }

  [Fact]
  public void FadesIn()
  {
    _animationPlayer.Setup(player => player.Play("fade_in", -1, 1, false));
    _menu.FadeIn();
    _animationPlayer.VerifyAll();
  }

  [Fact]
  public void FadesOut()
  {
    _animationPlayer.Setup(player => player.Play("fade_out", -1, 1, false));
    _menu.FadeOut();
    _animationPlayer.VerifyAll();
  }

  [Fact]
  public void OnSavePressed()
  {
    var called = false;
    _menu.Save += () => called = true;

    _menu.OnSavePressed();

    called.ShouldBeTrue();
  }

  [Fact]
  public void OnSaveStarted()
  {
    _saveOverlayAnimationPlayer
      .Setup(player => player.Play("save_fade_in", -1, 1, false));
    _menu.OnSaveStarted();

    _godot.GodotInstance.Iteration();

    _saveOverlayAnimationPlayer.VerifyAll();
  }

  [Fact]
  public void OnSaveFinished()
  {
    _saveOverlayAnimationPlayer
      .Setup(player => player.Play("save_fade_out", -1, 1, false));
    _menu.OnSaveCompleted();

    _godot.GodotInstance.Iteration();

    _saveOverlayAnimationPlayer.VerifyAll();
  }
}
