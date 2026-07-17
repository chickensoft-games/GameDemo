namespace GameDemo;

using Chickensoft.AutoInject;
using Chickensoft.GodotNodeInterfaces;
using Chickensoft.Introspection;
using Godot;

public interface IMenu : IControl
{
  void SetGameExists(bool gameExists);

  event Menu.NewGameEventHandler NewGame;
  event Menu.LoadGameEventHandler LoadGame;
  event Menu.DeleteGameEventHandler DeleteGame;
}

[Meta(typeof(IAutoNode))]
public partial class Menu : Control, IMenu
{
  public override void _Notification(int what) => this.Notify(what);

  #region Nodes
  [Node]
  public IButton NewGameButton { get; set; } = default!;
  [Node]
  public IButton LoadGameButton { get; set; } = default!;
  [Node]
  public IButton DeleteGameButton { get; set; } = default!;
  #endregion Nodes

  #region Signals
  [Signal]
  public delegate void NewGameEventHandler();
  [Signal]
  public delegate void LoadGameEventHandler();
  [Signal]
  public delegate void DeleteGameEventHandler();
  #endregion Signals

  public void OnReady()
  {
    NewGameButton.Pressed += OnNewGamePressed;
    LoadGameButton.Pressed += OnLoadGamePressed;
    DeleteGameButton.Pressed += OnDeleteGamePressed;
  }

  public void OnExitTree()
  {
    NewGameButton.Pressed -= OnNewGamePressed;
    LoadGameButton.Pressed -= OnLoadGamePressed;
    DeleteGameButton.Pressed -= OnDeleteGamePressed;
  }

  public void OnNewGamePressed() => EmitSignal(SignalName.NewGame);
  public void OnLoadGamePressed() => EmitSignal(SignalName.LoadGame);
  public void OnDeleteGamePressed() => EmitSignal(SignalName.DeleteGame);

  public void SetGameExists(bool gameExists)
  {
    LoadGameButton.Visible = gameExists;
    DeleteGameButton.Visible = gameExists;
  }
}
