namespace GameDemo;

using Chickensoft.GodotNodeInterfaces;
using Chickensoft.PowerUps;
using Godot;
using SuperNodes.Types;

public interface IPauseMenu : IControl {
  event PauseMenu.MainMenuEventHandler MainMenu;
  event PauseMenu.ResumeEventHandler Resume;
  event PauseMenu.TransitionCompletedEventHandler TransitionCompleted;

  void FadeIn();
  void FadeOut();
}

[SuperNode(typeof(AutoNode))]
public partial class PauseMenu : Control, IPauseMenu {
  public override partial void _Notification(int what);

  #region Nodes
  [Node]
  public IButton ResumeButton { get; set; } = default!;
  [Node]
  public IButton MainMenuButton { get; set; } = default!;
  [Node]
  public IAnimationPlayer AnimationPlayer { get; set; } = default!;
  #endregion Nodes

  #region Signals
  [Signal]
  public delegate void MainMenuEventHandler();
  [Signal]
  public delegate void ResumeEventHandler();
  [Signal]
  public delegate void TransitionCompletedEventHandler();
  #endregion Signals

  public void OnReady() {
    MainMenuButton.Pressed += OnMainMenuPressed;
    ResumeButton.Pressed += OnResumePressed;
    AnimationPlayer.AnimationFinished += OnAnimationFinished;
  }

  public void OnExitTree() {
    MainMenuButton.Pressed -= OnMainMenuPressed;
    ResumeButton.Pressed -= OnResumePressed;
    AnimationPlayer.AnimationFinished -= OnAnimationFinished;
  }

  public void OnMainMenuPressed() => EmitSignal(SignalName.MainMenu);
  public void OnResumePressed() => EmitSignal(SignalName.Resume);
  public void FadeIn() => AnimationPlayer.Play("fade_in");
  public void OnAnimationFinished(StringName name)
    => EmitSignal(SignalName.TransitionCompleted);
  public void FadeOut() => AnimationPlayer.Play("fade_out");
}
