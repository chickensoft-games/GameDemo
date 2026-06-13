namespace GameDemo.Tests;

using Moq;
using Shouldly;

[Collection("GodotHeadless")]
public class MapLogicStateTest
{
  private readonly StateTester _context;
  private readonly Mock<IGameRepo> _gameRepo = new ();
  private readonly MapLogicState _state = new();
  private readonly MapLogic.Data _data = new();

  public MapLogicStateTest()
  {
    _context = _state.Test();

    _context.Set(_gameRepo.Object);
    _context.Set(_data);
  }

  [Fact]
  public void GameLoadedFromSaveFile()
  {
    _state.On(new MapLogicState.Input.GameLoadedFromSaveFile(5));

    _gameRepo.Verify(repo => repo.SetNumCoinsCollected(5));
  }

  [Fact]
  public void OnCoinCollectionStarted()
  {
    var coin = new Mock<ICoin>();

    coin.Setup(coin => coin.Name).Returns("coin1");

    _state.OnCoinCollectionStarted(coin.Object);

    _data.CoinsBeingCollected.ShouldContain("coin1");
  }

  [Fact]
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
