namespace GameDemo.Tests;

using Chickensoft.GoDotTest;
using Godot;
using Moq;
using Shouldly;

public class CoinLogicTest : TestClass {
  private Mock<IGameRepo> _gameRepo = default!;
  private CoinLogic _logic = default!;
  private CoinLogic.Settings _settings = default!;
  private Mock<ICoin> _coin = default!;

  public CoinLogicTest(Node testScene) : base(testScene) { }

  [Setup]
  public void Setup() {
    _gameRepo = new();
    _coin = new();
    _settings = new CoinLogic.Settings(1.0f);
    _logic = new CoinLogic(_coin.Object, _settings, _gameRepo.Object);
  }

  [Test]
  public void Initializes() {
    _logic.Get<IGameRepo>().ShouldBe(_gameRepo.Object);
    _logic.Get<ICoin>().ShouldBe(_coin.Object);
    _logic.Get<CoinLogic.Settings>().ShouldBe(_settings);

    _logic
      .GetInitialState()
      .ShouldBeAssignableTo<CoinLogic.State.Idle>();
  }
}
