namespace GameDemo;

using Chickensoft.GodotNodeInterfaces;
using Godot;
using Chickensoft.AutoInject;
using Chickensoft.Introspection;

public interface IPauseMenu : IControl {
  event PauseMenu.MainMenuEventHandler MainMenu;
  event PauseMenu.ResumeEventHandler Resume;
  event PauseMenu.TransitionCompletedEventHandler TransitionCompleted;
  event PauseMenu.SaveEventHandler Save;

  void FadeIn();
  void FadeOut();

  void OnSaveStarted();
  void OnSaveCompleted();
}

[Meta(typeof(IAutoNode))]
public partial class PauseMenu : Control, IPauseMenu {
  public override void _Notification(int what) => this.Notify(what);

  #region Nodes

  [Node] public IButton ResumeButton { get; set; } = default!;
  [Node] public IButton SaveButton { get; set; } = default!;
  [Node] public IButton MainMenuButton { get; set; } = default!;
  [Node] public IAnimationPlayer AnimationPlayer { get; set; } = default!;
  [Node] public IAnimationPlayer SaveOverlayAnimationPlayer { get; set; } = default!;
  [Node] public IVBoxContainer SaveOverlay { get; set; } = default!;

  #endregion Nodes

  #region Signals

  [Signal]
  public delegate void MainMenuEventHandler();

  [Signal]
  public delegate void ResumeEventHandler();

  [Signal]
  public delegate void TransitionCompletedEventHandler();

  [Signal]
  public delegate void SaveEventHandler();

  #endregion Signals

  public void OnReady() {
    MainMenuButton.Pressed += OnMainMenuPressed;
    ResumeButton.Pressed += OnResumePressed;
    SaveButton.Pressed += OnSavePressed;
    AnimationPlayer.AnimationFinished += OnAnimationFinished;
  }

  public void OnExitTree() {
    MainMenuButton.Pressed -= OnMainMenuPressed;
    ResumeButton.Pressed -= OnResumePressed;
    SaveButton.Pressed -= OnSavePressed;
    AnimationPlayer.AnimationFinished -= OnAnimationFinished;
  }

  public void OnMainMenuPressed() => EmitSignal(SignalName.MainMenu);
  public void OnResumePressed() => EmitSignal(SignalName.Resume);
  public void OnSavePressed() => EmitSignal(SignalName.Save);

  public void FadeIn() => AnimationPlayer.Play("fade_in");

  public void OnAnimationFinished(StringName name)
    => EmitSignal(SignalName.TransitionCompleted);

  public void FadeOut() => AnimationPlayer.Play("fade_out");

  public void OnSaveStarted() => CallDeferred(nameof(Animate), "save_fade_in");
  public void OnSaveCompleted() =>
    CallDeferred(nameof(Animate), "save_fade_out");

  private void Animate(string animation) {
    SaveOverlayAnimationPlayer.Play(animation);
  }
}
