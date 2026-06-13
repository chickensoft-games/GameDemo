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
public class MenuTest
{
  private readonly Mock<IButton> _newGameButton = new();
  private readonly Mock<IButton> _loadGameButton = new();
  private readonly Menu _menu;

  public MenuTest()
  {
    _menu = new Menu
    {
      NewGameButton = _newGameButton.Object,
      LoadGameButton = _loadGameButton.Object
    };

    _menu._Notification(-1);
  }

  [Fact]
  public void Subscribes()
  {
    _menu.OnReady();
    _newGameButton.VerifyAdd(menu => menu.Pressed += _menu.OnNewGamePressed);

    _menu.OnExitTree();
    _newGameButton
      .VerifyRemove(menu => menu.Pressed -= _menu.OnNewGamePressed);
  }

  [Fact]
  public async Task SignalsNewGameButtonPressed()
  {
    var signal = _menu.ToSignal(_menu, Menu.SignalName.NewGame);

    _menu.OnNewGamePressed();

    await signal;

    signal.IsCompleted.ShouldBeTrue();
  }

  [Fact]
  public async Task SignalLoadGameButtonPressed()
  {
    var signal = _menu.ToSignal(_menu, Menu.SignalName.LoadGame);

    _menu.OnLoadGamePressed();

    await signal;

    signal.IsCompleted.ShouldBeTrue();
  }
}
