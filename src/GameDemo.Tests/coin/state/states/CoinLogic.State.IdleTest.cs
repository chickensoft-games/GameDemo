namespace GameDemo.Tests;

using Chickensoft.Collections;
using Moq;
using Shouldly;

[Collection("GodotHeadless")]
public class CoinLogicStateIdleTest
{
  private readonly StateTester _context;
  private readonly CoinLogicState.Idle _state = new();
  private readonly Mock<IAppRepo> _appRepo = new();
  private readonly Mock<ICoinCollector> _target = new();
  private readonly Mock<ICoin> _coin = new();
  private readonly CoinLogic.Data _data = new() { Target = "target_id" };

  private readonly EntityTable _entityTable = new();

  public CoinLogicStateIdleTest()
  {
    _entityTable.Set("target_id", _target.Object);

    _target.Setup(target => target.Name).Returns("target_id");

    _context = _state.Test();

    _context.Set(_coin.Object);
    _context.Set(_appRepo.Object);
    _context.Set(_data);
    _context.Set(_entityTable);
  }

  [Fact]
  public void GoesToCollectingOnStartCollection()
  {
    var next = _state.On(new CoinLogicState.Input.StartCollection(_target.Object));
    next.IsAssignableTo(typeof(CoinLogicState.Collecting)).ShouldBeTrue();
  }
}
