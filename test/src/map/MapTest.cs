namespace GameDemo.Tests;

using System.Collections.Generic;
using Chickensoft.AutoInject;
using Chickensoft.Collections;
using Chickensoft.GodotNodeInterfaces;
using Chickensoft.GoDotTest;
using Chickensoft.SaveFileBuilder;
using Godot;
using Moq;
using Shouldly;

public class MapTest : TestClass {
  private Mock<INode3D> _coins = default!;
  private Mock<IGameRepo> _gameRepo = default!;
  private Mock<IMapLogic> _mapLogic = default!;
  private EntityTable _entityTable = default!;
  private Mock<ISaveChunk<GameData>> _gameChunk = default!;
  private Mock<ISaveChunk<MapData>> _mapChunk = default!;
  private MapLogic.Data _data = default!;
  private Map _map = default!;

  public MapTest(Node testScene) : base(testScene) { }

  [Setup]
  public void Setup() {
    _coins = new();
    _gameRepo = new();
    _entityTable = new();
    _gameChunk = new();
    _mapLogic = new();
    _mapChunk = new();
    _data = new();

    _map = new Map {
      Coins = _coins.Object,
      MapLogic = _mapLogic.Object,
      MapChunk = _mapChunk.Object
    };

    _map.FakeDependency(_gameRepo.Object);
    _map.FakeDependency(_entityTable);
    _map.FakeDependency(_gameChunk.Object);

    _mapLogic.Setup(m => m.Get<MapLogic.Data>())
      .Returns(_data);
  }

  [Test]
  public void Initializes() {
    _map.Setup();
    _map.MapLogic.ShouldBeOfType<MapLogic>();
    (_map as IProvide<EntityTable>).Value().ShouldBe(_entityTable);
    (_map as IProvide<ISaveChunk<MapData>>).Value().ShouldBe(_mapChunk.Object);

    _map._Notification(-1);
  }

  [Test]
  public void GetCoinCount() {
    _coins.Setup(coins => coins.GetChildCount(false)).Returns(5);

    _map.GetCoinCount().ShouldBe(5);
  }

  [Test]
  public void PerformsSetup() {
    _map.OnResolved();

    _mapLogic.Verify(logic => logic.Set(It.IsAny<MapLogic.Data>()));
    _mapLogic.Verify(logic => logic.Set(_gameRepo.Object));
    _mapLogic.Verify(logic => logic.Start());
  }

  [Test]
  public void Saves() {
    var coin1 = new Mock<ICoin>();
    var coin2 = new Mock<ICoin>();

    coin2.Setup(c => c.CoinLogic).Returns(new CoinLogic());
    coin2.Setup(c => c.GlobalTransform).Returns(Transform3D.Identity);

    _entityTable.Set("coin1", coin1.Object);
    _entityTable.Set("coin2", coin2.Object);

    _map.OnResolved();

    _data.CoinsBeingCollected.Add("coin2");
    _data.CollectedCoinIds.Add("coin1");

    var mapData = _map.MapChunk.GetSaveData();

    mapData.CoinsBeingCollected.ShouldContainKey("coin2");
    mapData.CollectedCoinIds.ShouldContain("coin1");
  }

  [Test]
  public void Loads() {

    var mapData = new MapData() {
      CoinsBeingCollected = new Dictionary<string, CoinData> {
        ["coin2"] = new CoinData {
          StateMachine = new CoinLogic(),
          GlobalTransform = Transform3D.Identity
        }
      },
      CollectedCoinIds = new HashSet<string> { "coin1" }
    };

    var coinNode1 = new Mock<ICoin>();

    var coinNode2 = new Mock<ICoin>();
    var coinLogic2 = new Mock<ICoinLogic>();

    // Coin1 should be removed.

    _coins.Setup(c => c.GetNodeOrNullEx<INode>("coin1"))
      .Returns(coinNode1.Object);
    _coins.Setup(c => c.RemoveChildEx(coinNode1.Object));
    coinNode1.Setup(c => c.QueueFree());

    // Coin 2 should have its state and transform updated.

    _coins.Setup(c => c.GetNodeOrNullEx<INode>("coin2"))
      .Returns(coinNode2.Object);

    coinNode2.Setup(c => c.CoinLogic).Returns(coinLogic2.Object);
    coinLogic2.Setup(c => c.RestoreFrom(mapData.CoinsBeingCollected["coin2"].StateMachine));
    coinLogic2.Setup(c => c.Start());
    coinNode2.SetupSet(c => c.GlobalTransform = mapData.CoinsBeingCollected["coin2"].GlobalTransform);

    _map.OnResolved();

    _map.MapChunk.LoadSaveData(mapData);

    _coins.VerifyAll();
    coinNode1.VerifyAll();
    coinNode2.VerifyAll();
  }
}
