namespace GameDemo.Tests;
using Chickensoft.GoDotTest;
using Godot;
using Moq;
using Shouldly;

public class CoinLogicTest : TestClass {
  private Mock<IAppRepo> _appRepo = default!;
  private CoinLogic _logic = default!;
  private CoinLogic.Settings _settings = default!;
  private Mock<ICoin> _coin = default!;

  public CoinLogicTest(Node testScene) : base(testScene) { }

  [Setup]
  public void Setup() {
    _appRepo = new Mock<IAppRepo>();
    _coin = new Mock<ICoin>();
    _settings = new CoinLogic.Settings(1.0f);
    _logic = new CoinLogic(_coin.Object, _settings, _appRepo.Object);
  }

  [Test]
  public void Initializes() {
    _logic.Get<IAppRepo>().ShouldBe(_appRepo.Object);
    _logic.Get<ICoin>().ShouldBe(_coin.Object);
    _logic.Get<CoinLogic.Settings>().ShouldBe(_settings);

    var context = CoinLogic.CreateFakeContext();
    context.Set(_appRepo.Object);

    _logic
      .GetInitialState(context)
      .ShouldBeAssignableTo<CoinLogic.State.Idle>();
  }
}
