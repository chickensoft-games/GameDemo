namespace GameDemo.Tests;

using Chickensoft.Collections;
using Chickensoft.GoDotTest;
using Chickensoft.LogicBlocks;
using Godot;
using Moq;
using Shouldly;

public class CoinLogicStateIdleTest : TestClass {
  private IFakeContext _context = default!;
  private CoinLogic.State.Idle _state = default!;
  private Mock<IAppRepo> _appRepo = default!;
  private Mock<ICoinCollector> _target = default!;
  private Mock<ICoin> _coin = default!;
  private CoinLogic.Data _data = default!;

  private EntityTable _entityTable = default!;

  public CoinLogicStateIdleTest(Node testScene) : base(testScene) { }

  [Setup]
  public void Setup() {
    _state = new();
    _coin = new();
    _target = new();
    _appRepo = new();
    _data = new() { Target = "target_id" };
    _entityTable = new();
    _entityTable.Set("target_id", _target.Object);

    _target.Setup(target => target.Name).Returns("target_id");

    _context = _state.CreateFakeContext();

    _context.Set(_coin.Object);
    _context.Set(_appRepo.Object);
    _context.Set(_data);
    _context.Set(_entityTable);
  }

  [Test]
  public void GoesToCollectingOnStartCollection() {
    var next = _state.On(new CoinLogic.Input.StartCollection(_target.Object));
    next.State.ShouldBeAssignableTo<CoinLogic.State.Collecting>();
  }
}
