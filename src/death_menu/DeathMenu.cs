namespace GameDemo;

using Chickensoft.GodotNodeInterfaces;
using Chickensoft.PowerUps;
using Godot;
using SuperNodes.Types;

public interface IDeathMenu : IControl {
  event DeathMenu.TryAgainEventHandler TryAgain;
  event DeathMenu.MainMenuEventHandler MainMenu;
  event DeathMenu.TransitionCompletedEventHandler TransitionCompleted;

  void Animate();

  void FadeIn();
  void FadeOut();
}

[SuperNode(typeof(AutoNode))]
public partial class DeathMenu : Control, IDeathMenu {
  public override partial void _Notification(int what);

  [Signal]
  public delegate void TryAgainEventHandler();

  [Signal]
  public delegate void MainMenuEventHandler();

  [Signal]
  public delegate void TransitionCompletedEventHandler();

  #region Nodes

  [Node] public IButton TryAgainButton { get; set; } = default!;
  [Node] public IButton MainMenuButton { get; set; } = default!;
  [Node] public IAnimationPlayer AnimationPlayer { get; set; } = default!;
  [Node] public IAnimationPlayer FadeAnimationPlayer { get; set; } = default!;

  #endregion Nodes

  public void OnReady() {
    TryAgainButton.Pressed += OnTryAgainPressed;
    MainMenuButton.Pressed += OnMainMenuPressed;
    FadeAnimationPlayer.AnimationFinished += OnAnimationFinished;
  }

  public override void _Draw() {
    base._Draw();
    TryAgainButton.GrabFocus();
  }

  public void OnExitTree() {
    TryAgainButton.Pressed -= OnTryAgainPressed;
    MainMenuButton.Pressed -= OnMainMenuPressed;
    FadeAnimationPlayer.AnimationFinished -= OnAnimationFinished;
  }

  public void Animate() =>
    AnimationPlayer.Play("splotch");

  public void FadeIn() => FadeAnimationPlayer.Play("fade_in");

  public void FadeOut() => FadeAnimationPlayer.Play("fade_out");

  public void OnTryAgainPressed() => EmitSignal(SignalName.TryAgain);

  public void OnMainMenuPressed() => EmitSignal(SignalName.MainMenu);

  public void OnAnimationFinished(StringName animationName) =>
    EmitSignal(SignalName.TransitionCompleted);
}
