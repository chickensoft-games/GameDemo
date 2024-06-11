namespace GameDemo.Tests;

using System.Threading.Tasks;
using Chickensoft.GodotNodeInterfaces;
using Chickensoft.GoDotTest;
using Godot;
using Moq;
using Shouldly;

public class MenuTest : TestClass {
  private Mock<IButton> _newGameButton = default!;
  private Mock<IButton> _loadGameButton = default!;
  private Menu _menu = default!;

  public MenuTest(Node testScene) : base(testScene) { }

  [Setup]
  public void Setup() {
    _newGameButton = new Mock<IButton>();
    _loadGameButton = new Mock<IButton>();

    _menu = new Menu {
      NewGameButton = _newGameButton.Object,
      LoadGameButton = _loadGameButton.Object
    };

    _menu._Notification(-1);
  }

  [Test]
  public void Subscribes() {
    _menu.OnReady();
    _newGameButton.VerifyAdd(menu => menu.Pressed += _menu.OnNewGamePressed);

    _menu.OnExitTree();
    _newGameButton
      .VerifyRemove(menu => menu.Pressed -= _menu.OnNewGamePressed);
  }

  [Test]
  public async Task SignalsNewGameButtonPressed() {
    var signal = _menu.ToSignal(_menu, Menu.SignalName.NewGame);

    _menu.OnNewGamePressed();

    await signal;

    signal.IsCompleted.ShouldBeTrue();
  }

  [Test]
  public async Task SignalLoadGameButtonPressed() {
    var signal = _menu.ToSignal(_menu, Menu.SignalName.LoadGame);

    _menu.OnLoadGamePressed();

    await signal;

    signal.IsCompleted.ShouldBeTrue();
  }
}
