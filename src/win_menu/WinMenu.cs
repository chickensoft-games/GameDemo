namespace GameDemo;

using Chickensoft.GodotNodeInterfaces;
using Chickensoft.AutoInject;
using Godot;
using Chickensoft.Introspection;

public interface IWinMenu : IControl {
  event WinMenu.MainMenuEventHandler MainMenu;

  event WinMenu.TransitionCompletedEventHandler TransitionCompleted;

  void FadeIn();
  void FadeOut();
}

[Meta(typeof(IAutoNode))]
public partial class WinMenu : Control, IWinMenu {
  public override void _Notification(int what) => this.Notify(what);

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
