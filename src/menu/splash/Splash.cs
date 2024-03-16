namespace GameDemo;

using Chickensoft.AutoInject;
using Chickensoft.GodotNodeInterfaces;
using Chickensoft.PowerUps;
using Godot;
using SuperNodes.Types;

public interface ISplash : IControl;

[SuperNode(typeof(AutoNode), typeof(Dependent))]
public partial class Splash : Control, ISplash {
  public override partial void _Notification(int what);

  #region Nodes
  [Node]
  public IAnimationPlayer AnimationPlayer { get; set; } = default!;
  #endregion Nodes

  #region Dependencies
  [Dependency]
  public IAppRepo AppRepo => DependOn<IAppRepo>();
  #endregion Dependencies

  public void OnReady() {
    AnimationPlayer.AnimationFinished += OnAnimationFinished;
  }

  public void OnExitTree() {
    AnimationPlayer.AnimationFinished -= OnAnimationFinished;
  }

  public void OnAnimationFinished(StringName name)
    => AppRepo.SkipSplashScreen();

  public override void _Input(InputEvent @event) {
    // Clicking will skip the splash screen.
    if (@event is InputEventMouseButton mouseButton && mouseButton.Pressed) {
      AppRepo.SkipSplashScreen();
    }
  }
}
