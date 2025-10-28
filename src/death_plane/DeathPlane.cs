namespace GameDemo;

using Chickensoft.AutoInject;
using Chickensoft.Introspection;
using Godot;

[Meta(typeof(IAutoNode))]
public partial class DeathPlane : Area3D
{
  public override void _Notification(int what) => this.Notify(what);

  public void OnReady() => BodyEntered += OnBodyEntered;

  public void OnExitTree() => BodyEntered -= OnBodyEntered;

  public void OnBodyEntered(object body)
  {
    if (body is IKillable killable)
    {
      killable.Kill();
    }
  }
}
