namespace GameDemo;

using Godot;
using SuperNodes.Types;

[SuperNode]
public partial class DeathPlane : Area3D {
  public override partial void _Notification(int what);

  public void OnReady() => BodyEntered += OnBodyEntered;

  public void OnExitTree() => BodyEntered -= OnBodyEntered;

  public void OnBodyEntered(object body) {
    if (body is IKillable killable) {
      killable.Kill();
    }
  }
}
