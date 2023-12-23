namespace GameDemo.Tests;

using Chickensoft.GoDotTest;
using Chickensoft.LogicBlocks;
using Godot;
using Moq;
using Shouldly;

public class CoinLogicStateIdleTest : TestClass {
  private IFakeContext _context = default!;
  private CoinLogic.State.Idle _state = default!;
  private Mock<IAppRepo> _appRepo = default!;
  private Mock<ICoin> _coin = default!;

  public CoinLogicStateIdleTest(Node testScene) : base(testScene) { }

  [Setup]
  public void Setup() {
    _coin = new();
    _appRepo = new();

    _state = new();
    _context = _state.CreateFakeContext();

    _context.Set(_coin.Object);
    _context.Set(_appRepo.Object);
  }

  [Test]
  public void GoesToCollectingOnStartCollection() {
    var next = _state.On(new CoinLogic.Input.StartCollection());
    next.ShouldBeAssignableTo<CoinLogic.State.Collecting>();
  }
}
