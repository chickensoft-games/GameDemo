namespace GameDemo;

using Chickensoft.GodotNodeInterfaces;
using Chickensoft.PowerUps;
using Godot;
using SuperNodes.Types;
using Chickensoft.AutoInject;

public interface IMenu : IControl {
  event Menu.StartEventHandler Start;
}

[SuperNode(typeof(AutoNode), typeof(Dependent))]
public partial class Menu : Control, IMenu {
  public override partial void _Notification(int what);

  #region Nodes
  [Node]
  public IButton StartButton { get; set; } = default!;
  #endregion Nodes

  #region Signals
  [Signal]
  public delegate void StartEventHandler();
  #endregion Signals

  public override void _Input(InputEvent @event) {
    if (Input.IsActionJustPressed("ui_accept")) {
      OnStartPressed();
    }
  }

  public void OnReady() => StartButton.Pressed += OnStartPressed;

  public void OnExitTree() => StartButton.Pressed -= OnStartPressed;

  public void OnStartPressed() => EmitSignal(SignalName.Start);
}
