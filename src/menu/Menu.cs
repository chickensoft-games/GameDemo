namespace GameDemo;

using Chickensoft.GodotNodeInterfaces;
using Chickensoft.AutoInject;
using Godot;
using Chickensoft.Introspection;

public interface IMenu : IControl {
  event Menu.NewGameEventHandler NewGame;
  event Menu.LoadGameEventHandler LoadGame;
}

[Meta(typeof(IAutoNode))]
public partial class Menu : Control, IMenu {
  public override void _Notification(int what) => this.Notify(what);

  #region Nodes
  [Node]
  public IButton NewGameButton { get; set; } = default!;
  [Node]
  public IButton LoadGameButton { get; set; } = default!;
  [Node]
  public ILabel SteamUsernameLabel { get; set; } = default!;
  #endregion Nodes

  #region State

  [Dependency] public INetworkingRepo NetworkingRepo => this.DependOn<INetworkingRepo>();

  #endregion

  #region Signals
  [Signal]
  public delegate void NewGameEventHandler();
  [Signal]
  public delegate void LoadGameEventHandler();
  #endregion Signals

  public void OnReady() {
    NewGameButton.Pressed += OnNewGamePressed;
    LoadGameButton.Pressed += OnLoadGamePressed;
  }

  public void OnResolved() {
    NetworkingRepo.PersonaName.Sync += (name) => SteamUsernameLabel.Text = $"Steam Username: {name}";
  }

  public void OnExitTree() {
    NewGameButton.Pressed -= OnNewGamePressed;
    LoadGameButton.Pressed -= OnLoadGamePressed;
  }

  public void OnNewGamePressed() => EmitSignal(SignalName.NewGame);
  public void OnLoadGamePressed() => EmitSignal(SignalName.LoadGame);
}
