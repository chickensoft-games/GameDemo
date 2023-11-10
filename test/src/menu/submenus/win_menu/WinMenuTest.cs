namespace GameDemo.Tests;

using System.Threading.Tasks;
using Chickensoft.GodotNodeInterfaces;
using Chickensoft.GoDotTest;
using Godot;
using Moq;
using Shouldly;

public class WinMenuTest : TestClass {
  private Mock<IButton> _mainMenuButton = default!;
  private WinMenu _menu = default!;

  public WinMenuTest(Node testScene) : base(testScene) { }

  [Setup]
  public void Setup() {
    _mainMenuButton = new Mock<IButton>();
    _menu = new WinMenu {
      MainMenuButton = _mainMenuButton.Object
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
    var signal = _menu.ToSignal(_menu, WinMenu.SignalName.MainMenu);

    _menu.OnMainMenuPressed();

    await signal;

    signal.IsCompleted.ShouldBeTrue();
  }
}
