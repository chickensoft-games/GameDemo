namespace GameDemo.Tests;

using System.Threading.Tasks;
using Chickensoft.GodotNodeInterfaces;
using Chickensoft.GoDotTest;
using Godot;
using Moq;
using Shouldly;

public class MenuTest : TestClass {
  private Mock<IAppRepo> _appRepo = default!;

  private Mock<IButton> _startButton = default!;
  private Menu _menu = default!;

  public MenuTest(Node testScene) : base(testScene) { }

  [Setup]
  public void Setup() {
    _appRepo = new Mock<IAppRepo>();
    _startButton = new Mock<IButton>();
    _menu = new Menu {
      StartButton = _startButton.Object
    };
    _menu.FakeDependency(_appRepo.Object);
  }

  [Test]
  public void Subscribes() {
    _menu.OnReady();
    _startButton.VerifyAdd(menu => menu.Pressed += _menu.OnStartPressed);

    _menu.OnExitTree();
    _startButton
      .VerifyRemove(menu => menu.Pressed -= _menu.OnStartPressed);

  }

  [Test]
  public async Task SignalsStartButtonPressed() {
    var signal = _menu.ToSignal(_menu, Menu.SignalName.Start);

    _menu.OnStartPressed();

    await signal;

    signal.IsCompleted.ShouldBeTrue();
  }

  [Test]
  public async Task UIAcceptActionPressed() {
    var signal = _menu.ToSignal(_menu, Menu.SignalName.Start);

    var uiAccept = new InputEventAction() {
      Action = "ui_accept",
      Pressed = true
    };

    _menu._Input(uiAccept);

    await signal;

    signal.IsCompleted.ShouldBeTrue();

  }
}
