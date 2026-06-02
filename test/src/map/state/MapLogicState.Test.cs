namespace GameDemo.Tests;

using Chickensoft.GoDotTest;
using Godot;
using Moq;
using Shouldly;

public class MapLogicStateTest : TestClass
{
  private StateTester _context = default!;
  private Mock<IGameRepo> _gameRepo = default!;
  private MapLogicState _state = default!;
  private MapLogic.Data _data = default!;

  public MapLogicStateTest(Node testScene) : base(testScene) { }

  [Setup]
  public void Setup()
  {
    _gameRepo = new ();
    _data = new MapLogic.Data();

    _state = new MapLogicState();
    _context = _state.Test();

    _context.Set(_gameRepo.Object);
    _context.Set(_data);
  }

  [Test]
  public void GameLoadedFromSaveFile()
  {
    _state.On(new MapLogicState.Input.GameLoadedFromSaveFile(5));

    _gameRepo.Verify(repo => repo.SetNumCoinsCollected(5));
  }

  [Test]
  public void OnCoinCollectionStarted()
  {
    var coin = new Mock<ICoin>();

    coin.Setup(coin => coin.Name).Returns("coin1");

    _state.OnCoinCollectionStarted(coin.Object);

    _data.CoinsBeingCollected.ShouldContain("coin1");
  }

  [Test]
  public void OnCoinCollectionCompleted()
  {
    var coin = new Mock<ICoin>();

    coin.Setup(coin => coin.Name).Returns("coin1");

    _state.OnCoinCollectionStarted(coin.Object);
    _state.OnCoinCollectionCompleted(coin.Object);

    _data.CoinsBeingCollected.ShouldNotContain("coin1");
    _data.CollectedCoinIds.ShouldContain("coin1");
  }
}
