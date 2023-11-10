namespace GameDemo.Tests;

using Chickensoft.GoDotTest;
using Godot;
using Moq;
using Shouldly;

public partial class DeathPlaneTest : TestClass {
  private DeathPlane _plane = default!;

  public DeathPlaneTest(Node testScene) : base(testScene) { }

  [Setup]
  public void Setup() { _plane = new(); }

  [Test]
  public void InitializesAndCleansUp() {
    Should.NotThrow(() => {
      _plane.OnReady();
      _plane.OnExitTree();
    });
  }

  [Test]
  public void KillsKillableObjects() {
    var killable = new Mock<IKillable>();
    killable.Setup(k => k.Kill());

    _plane.OnBodyEntered(killable.Object);

    killable.VerifyAll();
  }
}
