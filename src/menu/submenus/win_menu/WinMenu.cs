namespace GameDemo;

using Chickensoft.GodotNodeInterfaces;
using Chickensoft.PowerUps;
using Godot;
using SuperNodes.Types;

public interface IWinMenu : IControl {
  event WinMenu.MainMenuEventHandler MainMenu;
}

[SuperNode(typeof(AutoNode))]
public partial class WinMenu : Control, IWinMenu {
  public override partial void _Notification(int what);

  #region Nodes
  [Node]
  public IButton MainMenuButton { get; set; } = default!;
  #endregion Nodes

  #region Signals
  [Signal]
  public delegate void MainMenuEventHandler();
  #endregion Signals

  public void OnReady() => MainMenuButton.Pressed += OnMainMenuPressed;

  public void OnExitTree() => MainMenuButton.Pressed -= OnMainMenuPressed;

  public void OnMainMenuPressed() => EmitSignal(SignalName.MainMenu);
}
