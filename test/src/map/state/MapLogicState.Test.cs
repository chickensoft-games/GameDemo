namespace GameDemo.Tests;

using Chickensoft.GoDotTest;
using Godot;
using Moq;
using Shouldly;

public class MapLogicStateTest : TestClass
{
  private StateTester _context = default!;
  private IGameRepo _gameRepo = default!;
  private MapLogic.BaseState _baseState = default!;
  private MapLogic.Data _data = default!;

  public MapLogicStateTest(Node testScene) : base(testScene) { }

  [Setup]
  public void Setup()
  {
    _gameRepo = new GameRepo();
    _data = new MapLogic.Data();
    _baseState = new MapLogic.BaseState();
    _context = _baseState.Test();
    _context.Set(_gameRepo);
    _context.Set(_data);
    MapLogic.SetupSubscriptions(_gameRepo, () => _baseState);
  }

  [Cleanup]
  public void Cleanup() => _gameRepo.Dispose();

  [Test]
  public void GameLoadedFromSaveFile()
  {
    _baseState.On(new MapLogic.Input.GameLoadedFromSaveFile(5));

    _gameRepo.NumCoinsCollected.Value.ShouldBe(5);
  }

  [Test]
  public void OnCoinCollectionStarted()
  {
    var coin = new Mock<ICoin>();
    coin.Setup(c => c.Name).Returns("coin1");

    _gameRepo.StartCoinCollection(coin.Object);

    _data.CoinsBeingCollected.ShouldContain("coin1");
  }

  [Test]
  public void OnCoinCollectionCompleted()
  {
    var coin = new Mock<ICoin>();
    coin.Setup(c => c.Name).Returns("coin1");

    _gameRepo.StartCoinCollection(coin.Object);
    _gameRepo.OnFinishCoinCollection(coin.Object);

    _data.CoinsBeingCollected.ShouldNotContain("coin1");
    _data.CollectedCoinIds.ShouldContain("coin1");
  }
}
