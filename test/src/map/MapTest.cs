namespace GameDemo.Tests;

using Chickensoft.GodotNodeInterfaces;
using Chickensoft.GoDotTest;
using Godot;
using Moq;
using Shouldly;

public class MapTest : TestClass {
  private Mock<INode3D> _coins = default!;
  private Map _map = default!;

  public MapTest(Node testScene) : base(testScene) { }

  [Setup]
  public void Setup() {
    _coins = new Mock<INode3D>();
    _map = new Map {
      Coins = _coins.Object
    };
  }

  [Test]
  public void GetCoinCount() {
    _coins.Setup(coins => coins.GetChildCount(false)).Returns(5);

    _map.GetCoinCount().ShouldBe(5);
  }
}
