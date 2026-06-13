namespace GameDemo.Tests;

using System.Diagnostics.CodeAnalysis;
using Moq;
using Shouldly;

[
  SuppressMessage(
    "Design",
    "CA1001",
    Justification = "Disposable field is a Godot object; Godot will dispose"
  )
]
[Collection("GodotHeadless")]
public partial class DeathPlaneTest
{
  private readonly DeathPlane _plane = new();

  [Fact]
  public void InitializesAndCleansUp() => Should.NotThrow(() =>
  {
    _plane.OnReady();
    _plane.OnExitTree();
    _plane._Notification(-1);
  });

  [Fact]
  public void KillsKillableObjects()
  {
    var killable = new Mock<IKillable>();
    killable.Setup(k => k.Kill());

    _plane.OnBodyEntered(killable.Object);

    killable.VerifyAll();
  }
}
