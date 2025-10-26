namespace GameDemo.Tests;

using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
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
public class WinMenuTest : TestClass
{
  private Mock<IButton> _mainMenuButton = default!;
  private Mock<IAnimationPlayer> _animationPlayer = default!;
  private WinMenu _menu = default!;

  public WinMenuTest(Node testScene) : base(testScene) { }

  [Setup]
  public void Setup()
  {
    _mainMenuButton = new Mock<IButton>();
    _animationPlayer = new Mock<IAnimationPlayer>();
    _menu = new WinMenu
    {
      MainMenuButton = _mainMenuButton.Object,
      AnimationPlayer = _animationPlayer.Object
    };

    _menu._Notification(-1);
  }

  [Test]
  public void Subscribes()
  {
    _menu.OnReady();
    _mainMenuButton.VerifyAdd(menu => menu.Pressed += _menu.OnMainMenuPressed);

    _menu.OnExitTree();
    _mainMenuButton
      .VerifyRemove(menu => menu.Pressed -= _menu.OnMainMenuPressed);
  }

  [Test]
  public async Task SignalsMainMenuButtonPressed()
  {
    var signal = _menu.ToSignal(_menu, WinMenu.SignalName.MainMenu);

    _menu.OnMainMenuPressed();

    await signal;

    signal.IsCompleted.ShouldBeTrue();
  }

  [Test]
  public void FadeIn()
  {
    _menu.FadeIn();
    _animationPlayer.Verify(player => player.Play("fade_in", -1, 1, false));
  }

  [Test]
  public void FadeOut()
  {
    _menu.FadeOut();
    _animationPlayer.Verify(player => player.Play("fade_out", -1, 1, false));
  }

  [Test]
  public void OnAnimationFinished()
  {
    var called = false;
    _menu.TransitionCompleted += () => called = true;

    _menu.OnAnimationFinished("fade_in");

    called.ShouldBeTrue();
  }
}
