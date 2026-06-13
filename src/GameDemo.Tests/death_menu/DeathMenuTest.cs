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
    Justification = "Disposable field is a Godot object; Godot will dispose"
  )
]
[Collection("GodotHeadless")]
public class DeathMenuTest
{
  private readonly Mock<IButton> _mainMenuButton = new();
  private readonly Mock<IButton> _tryAgainButton = new();
  private readonly Mock<IAnimationPlayer> _animationPlayer = new();
  private readonly Mock<IAnimationPlayer> _fadeAnimationPlayer = new();
  private readonly DeathMenu _menu;

  public DeathMenuTest()
  {
    _menu = new DeathMenu
    {
      MainMenuButton = _mainMenuButton.Object,
      TryAgainButton = _tryAgainButton.Object,
      AnimationPlayer = _animationPlayer.Object,
      FadeAnimationPlayer = _fadeAnimationPlayer.Object
    };

    _menu._Notification(-1);
  }

  [Fact]
  public void Subscribes()
  {
    _menu.OnReady();
    _mainMenuButton.VerifyAdd(menu => menu.Pressed += _menu.OnMainMenuPressed);
    _tryAgainButton.VerifyAdd(menu => menu.Pressed += _menu.OnTryAgainPressed);

    _menu.OnExitTree();
    _mainMenuButton
      .VerifyRemove(menu => menu.Pressed -= _menu.OnMainMenuPressed);
    _tryAgainButton
      .VerifyRemove(menu => menu.Pressed -= _menu.OnTryAgainPressed);
  }

  [Fact]
  public async Task SignalsMainMenuButtonPressed()
  {
    var signal = _menu.ToSignal(_menu, DeathMenu.SignalName.MainMenu);

    _menu.OnMainMenuPressed();

    await signal;

    signal.IsCompleted.ShouldBeTrue();
  }

  [Fact]
  public async Task SignalsTryAgainButtonPressed()
  {
    var signal = _menu.ToSignal(_menu, DeathMenu.SignalName.TryAgain);

    _menu.OnTryAgainPressed();

    await signal;

    signal.IsCompleted.ShouldBeTrue();
  }

  [Fact]
  public void Animates()
  {
    _animationPlayer
      .Setup(player => player.Play("splotch", -1, 1, false));

    _menu.Animate();

    _animationPlayer.VerifyAll();
  }

  [Fact]
  public void FadeIn()
  {
    _fadeAnimationPlayer
      .Setup(player => player.Play("fade_in", -1, 1, false));

    _menu.FadeIn();

    _fadeAnimationPlayer.VerifyAll();
  }

  [Fact]
  public void FadeOut()
  {
    _fadeAnimationPlayer
      .Setup(player => player.Play("fade_out", -1, 1, false));

    _menu.FadeOut();

    _fadeAnimationPlayer.VerifyAll();
  }

  [Fact]
  public void OnAnimationFinished()
  {
    var called = false;
    _menu.TransitionCompleted += () => called = true;

    _menu.OnAnimationFinished("animation");

    called.ShouldBeTrue();
  }
}
