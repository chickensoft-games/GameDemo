namespace GameDemo.Tests;

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Chickensoft.AutoInject;
using Chickensoft.Collections;
using Chickensoft.GodotNodeInterfaces;
using Chickensoft.GoDotTest;
using Godot;
using Moq;
using Shouldly;

[
  SuppressMessage(
    "Design",
    "CA1001",
    Justification = "Disposable field is Godot object; Godot will dispose"
  )
]
public class MapTest : TestClass
{
  private Mock<INode3D> _coins = default!;
  private Mock<IGameRepo> _gameRepo = default!;
  private Mock<IMapLogic> _mapLogic = default!;
  private EntityTable _entityTable = default!;
  private MapLogic.Data _data = default!;
  private Map _map = default!;

  public MapTest(Node testScene) : base(testScene) { }

  [Setup]
  public void Setup()
  {
    _coins = new();
    _gameRepo = new();
    _entityTable = new();
    _mapLogic = new();
    _data = new();

    _map = new Map
    {
      Coins = _coins.Object,
      MapLogic = _mapLogic.Object,
    };

    _map.FakeDependency(_gameRepo.Object);
    _map.FakeDependency(_entityTable);

    _mapLogic.Setup(m => m.Get<MapLogic.Data>())
      .Returns(_data);
  }

  [Test]
  public void Initializes()
  {
    _map.Setup();
    _map.MapLogic.ShouldBeOfType<MapLogic>();
    (_map as IProvide<EntityTable>).Value().ShouldBe(_entityTable);

    _map._Notification(-1);
  }

  [Test]
  public void GetCoinCount()
  {
    _coins.Setup(coins => coins.GetChildCount(false)).Returns(5);

    _map.GetCoinCount().ShouldBe(5);
  }

  [Test]
  public void PerformsSetup()
  {
    _map.OnResolved();

    _mapLogic.Verify(logic => logic.Set(It.IsAny<MapLogic.Data>()));
    _mapLogic.Verify(logic => logic.Set(_gameRepo.Object));
    _mapLogic.Verify(logic => logic.Start<MapLogicState>(true));
  }

  [Test]
  public void Saves()
  {
    var coin1 = new Mock<ICoin>();
    var coin2 = new Mock<ICoin>();

    _entityTable.Set("coin1", coin1.Object);
    _entityTable.Set("coin2", coin2.Object);

    _data.CoinsBeingCollected.Add("coin2");
    _data.CollectedCoinIds.Add("coin1");

    var mapData = _map.Save();

    mapData.CoinsBeingCollected.ShouldContainKey("coin2");
    mapData.CollectedCoinIds.ShouldContain("coin1");
    coin1.Verify(c => c.Save(), Times.Never());
    coin2.Verify(c => c.Save());
  }

  [Test]
  public void Loads()
  {
    var mapData = new MapData()
    {
      CoinsBeingCollected = new Dictionary<string, CoinData>
      {
        ["coin2"] = new CoinData
        {
          StateMachine = logic.Save(),
          GlobalTransform = Transform3D.Identity
        }
      },
      CollectedCoinIds = ["coin1"]
    };

    var coinNode1 = new Mock<ICoin>();

    var coinNode2 = new Mock<ICoin>();
    var coinLogic2 = new Mock<ICoinLogic>();

    // Coin1 should be removed.
    _coins.Setup(c => c.GetNodeOrNullEx<INode>("coin1")).Returns(coinNode1.Object);
    _coins.Setup(c => c.RemoveChildEx(coinNode1.Object));
    coinNode1.Setup(c => c.QueueFree());

    // Coin 2 should have its state and transform updated.
    _coins.Setup(c => c.GetNodeOrNullEx<INode>("coin2")).Returns(coinNode2.Object);
    coinNode2.Setup(c => c.Load(mapData.CoinsBeingCollected["coin2"]));

    _map.Load(mapData);

    _coins.VerifyAll();
    coinNode1.VerifyAll();
    coinNode2.VerifyAll();
  }
}
