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
[Collection("GodotHeadless")]
public class WinMenuTest
{
  private readonly Mock<IButton> _mainMenuButton = new();
  private readonly Mock<IAnimationPlayer> _animationPlayer = new();
  private readonly WinMenu _menu;

  public WinMenuTest()
  {
    _menu = new WinMenu
    {
      MainMenuButton = _mainMenuButton.Object,
      AnimationPlayer = _animationPlayer.Object
    };

    _menu._Notification(-1);
  }

  [Fact]
  public void Subscribes()
  {
    _menu.OnReady();
    _mainMenuButton.VerifyAdd(menu => menu.Pressed += _menu.OnMainMenuPressed);

    _menu.OnExitTree();
    _mainMenuButton
      .VerifyRemove(menu => menu.Pressed -= _menu.OnMainMenuPressed);
  }

  [Fact]
  public async Task SignalsMainMenuButtonPressed()
  {
    var signal = _menu.ToSignal(_menu, WinMenu.SignalName.MainMenu);

    _menu.OnMainMenuPressed();

    await signal;

    signal.IsCompleted.ShouldBeTrue();
  }

  [Fact]
  public void FadeIn()
  {
    _menu.FadeIn();
    _animationPlayer.Verify(player => player.Play("fade_in", -1, 1, false));
  }

  [Fact]
  public void FadeOut()
  {
    _menu.FadeOut();
    _animationPlayer.Verify(player => player.Play("fade_out", -1, 1, false));
  }

  [Fact]
  public void OnAnimationFinished()
  {
    var called = false;
    _menu.TransitionCompleted += () => called = true;

    _menu.OnAnimationFinished("fade_in");

    called.ShouldBeTrue();
  }
}
