namespace GameDemo.Tests;

using Chickensoft.GoDotTest;
using Chickensoft.LogicBlocks;
using Godot;
using Moq;
using Shouldly;

public class MapLogicStateTest : TestClass
{
  private IFakeContext _context = default!;
  private Mock<IGameRepo> _gameRepo = default!;
  private MapLogic.State _state = default!;
  private MapLogic.Data _data = default!;

  public MapLogicStateTest(Node testScene) : base(testScene) { }

  [Setup]
  public void Setup()
  {
    _gameRepo = new Mock<IGameRepo>();
    _data = new MapLogic.Data();

    _state = new MapLogic.State();
    _context = _state.CreateFakeContext();

    _context.Set(_gameRepo.Object);
    _context.Set(_data);
  }

  [Test]
  public void Subscribes()
  {
    _state.Attach(_context);

    _gameRepo.VerifyAdd(
      repo => repo.CoinCollectionStarted += _state.OnCoinCollectionStarted
    );
    _gameRepo.VerifyAdd(
      repo => repo.CoinCollectionCompleted += _state.OnCoinCollectionCompleted
    );

    _state.Detach();

    _gameRepo.VerifyRemove(
      repo => repo.CoinCollectionStarted -= _state.OnCoinCollectionStarted
    );
    _gameRepo.VerifyRemove(
      repo => repo.CoinCollectionCompleted -= _state.OnCoinCollectionCompleted
    );
  }

  [Test]
  public void GameLoadedFromSaveFile()
  {
    _state.On(new MapLogic.Input.GameLoadedFromSaveFile(5));

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
