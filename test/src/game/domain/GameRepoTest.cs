namespace GameDemo.Tests;

using Chickensoft.GoDotCollections;
using Chickensoft.GoDotTest;
using Godot;
using Shouldly;

public class GameRepoTest : TestClass {
  private AutoProp<Vector3> _playerGlobalPosition = default!;
  private AutoProp<Basis> _cameraBasis = default!;

  private GameRepo _repo = default!;

  public GameRepoTest(Node testScene) : base(testScene) { }

  [Setup]
  public void Setup() {
    _playerGlobalPosition = new(Vector3.Zero);
    _cameraBasis = new(Basis.Identity);

    _repo = new(
      _playerGlobalPosition,
      _cameraBasis
    );
  }

  [Cleanup]
  public void Cleanup() => _repo.Dispose();

  [Test]
  public void Initializes() {
    var repo = new GameRepo();
    repo.ShouldBeAssignableTo<IGameRepo>();
  }

  [Test]
  public void SetPlayerGlobalPosition() {
    _repo.SetPlayerGlobalPosition(Vector3.One);

    _repo.PlayerGlobalPosition.Value.ShouldBe(Vector3.One);
  }

  [Test]
  public void SetCameraBasis() {
    _repo.SetCameraBasis(Basis.Identity);

    _repo.CameraBasis.Value.ShouldBe(Basis.Identity);
  }

  [Test]
  public void GlobalCameraDirection() {
    _repo.SetCameraBasis(Basis.Identity);

    _repo.GlobalCameraDirection.ShouldBe(Vector3.Forward);
  }
}
