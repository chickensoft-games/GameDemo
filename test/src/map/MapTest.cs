namespace GameDemo.Tests;

using Chickensoft.GodotNodeInterfaces;
using Chickensoft.GoDotTest;
using Godot;
using Moq;
using Shouldly;

public class MapTest : TestClass {
  private Mock<INode3D> _coins = default!;
  private Mock<IGameRepo> _gameRepo = default!;
  private Map _map = default!;

  public MapTest(Node testScene) : base(testScene) { }

  [Setup]
  public void Setup() {
    _coins = new();
    _gameRepo = new();
    _map = new Map { Coins = _coins.Object };
    _map.FakeDependency(_gameRepo.Object);
  }

  [Test]
  public void Initializes() => _map.GameRepo.ShouldBe(_gameRepo.Object);

  [Test]
  public void GetCoinCount() {
    _coins.Setup(coins => coins.GetChildCount(false)).Returns(5);

    _map.GetCoinCount().ShouldBe(5);
  }
}
