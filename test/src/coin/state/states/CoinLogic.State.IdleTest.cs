namespace GameDemo.Tests;

using Chickensoft.GoDotTest;
using Godot;
using Moq;
using Shouldly;

public class CoinLogicStateIdleTest : TestClass {
  private CoinLogic.IFakeContext _context = default!;
  private CoinLogic.State.Idle _state = default!;
  private Mock<IAppRepo> _appRepo = default!;
  private Mock<ICoin> _coin = default!;

  public CoinLogicStateIdleTest(Node testScene) : base(testScene) { }

  [Setup]
  public void Setup() {
    _context = CoinLogic.CreateFakeContext();

    _coin = new();
    _appRepo = new();

    _context.Set(_coin.Object);
    _context.Set(_appRepo.Object);

    _state = new(_context);
  }

  [Test]
  public void GoesToCollectingOnStartCollection() {
    var next = _state.On(new CoinLogic.Input.StartCollection());
    next.ShouldBeAssignableTo<CoinLogic.State.Collecting>();
  }
}
