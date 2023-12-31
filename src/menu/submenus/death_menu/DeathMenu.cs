namespace GameDemo;

using Chickensoft.GodotNodeInterfaces;
using Chickensoft.PowerUps;
using Godot;
using SuperNodes.Types;

public interface IDeathMenu : IControl {
  event DeathMenu.TryAgainEventHandler TryAgain;
  event DeathMenu.MainMenuEventHandler MainMenu;

  void Animate();
}

[SuperNode(typeof(AutoNode))]
public partial class DeathMenu : Control, IDeathMenu {
  public override partial void _Notification(int what);

  [Signal]
  public delegate void TryAgainEventHandler();

  [Signal]
  public delegate void MainMenuEventHandler();

  #region Nodes

  [Node] public IButton TryAgainButton { get; set; } = default!;
  [Node] public IButton MainMenuButton { get; set; } = default!;
  [Node] public IAnimationPlayer AnimationPlayer { get; set; } = default!;

  #endregion Nodes

  public void OnReady() {
    TryAgainButton.Pressed += OnTryAgainPressed;
    MainMenuButton.Pressed += OnMainMenuPressed;
  }

  public void OnExitTree() {
    TryAgainButton.Pressed -= OnTryAgainPressed;
    MainMenuButton.Pressed -= OnMainMenuPressed;
  }

  public void Animate() =>
    AnimationPlayer.Play("splotch");

  public void OnTryAgainPressed() => EmitSignal(SignalName.TryAgain);

  public void OnMainMenuPressed() => EmitSignal(SignalName.MainMenu);
}
