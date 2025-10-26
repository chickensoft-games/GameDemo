namespace GameDemo;

using Chickensoft.AutoInject;
using Chickensoft.GodotNodeInterfaces;
using Chickensoft.Introspection;
using Godot;

public interface ISplash : IControl;

[Meta(typeof(IAutoNode))]
public partial class Splash : Control, ISplash
{
  public override void _Notification(int what) => this.Notify(what);

  #region Nodes
  [Node]
  public IAnimationPlayer AnimationPlayer { get; set; } = default!;
  #endregion Nodes

  #region Dependencies
  [Dependency]
  public IAppRepo AppRepo => this.DependOn<IAppRepo>();
  #endregion Dependencies

  public void OnReady() =>
    AnimationPlayer.AnimationFinished += OnAnimationFinished;

  public void OnExitTree()
    => AnimationPlayer.AnimationFinished -= OnAnimationFinished;

  public void OnAnimationFinished(StringName name)
    => AppRepo.SkipSplashScreen();

  public override void _Input(InputEvent @event)
  {
    // Clicking will skip the splash screen.
    if (@event is InputEventMouseButton mouseButton && mouseButton.Pressed)
    {
      AppRepo.SkipSplashScreen();
    }
  }
}
