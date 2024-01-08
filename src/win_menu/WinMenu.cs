namespace GameDemo;

using Chickensoft.GodotNodeInterfaces;
using Chickensoft.PowerUps;
using Godot;
using SuperNodes.Types;

public interface IWinMenu : IControl {
  event WinMenu.MainMenuEventHandler MainMenu;

  event WinMenu.TransitionCompletedEventHandler TransitionCompleted;

  void FadeIn();
  void FadeOut();
}

[SuperNode(typeof(AutoNode))]
public partial class WinMenu : Control, IWinMenu {
  public override partial void _Notification(int what);

  #region Nodes

  [Node] public IButton MainMenuButton { get; set; } = default!;
  [Node] public IAnimationPlayer AnimationPlayer { get; set; } = default!;

  #endregion Nodes

  #region Signals

  [Signal]
  public delegate void MainMenuEventHandler();

  [Signal]
  public delegate void TransitionCompletedEventHandler();

  #endregion Signals

  public void OnReady() => MainMenuButton.Pressed += OnMainMenuPressed;

  public void OnExitTree() => MainMenuButton.Pressed -= OnMainMenuPressed;

  public void OnMainMenuPressed() => EmitSignal(SignalName.MainMenu);

  public void FadeIn() => AnimationPlayer.Play("fade_in");

  public void OnAnimationFinished(StringName name)
    => EmitSignal(SignalName.TransitionCompleted);

  public void FadeOut() => AnimationPlayer.Play("fade_out");
}
