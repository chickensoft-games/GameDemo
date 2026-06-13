namespace GameDemo.Tests;

using Moq;
using Shouldly;

public class JumpshroomLogicStateIdleTest
{
  private readonly StateTester _context;
  private readonly Mock<IAppRepo> _appRepo = new();
  private readonly JumpshroomLogicState.Idle _state = new();

  public JumpshroomLogicStateIdleTest()
  {
    _context = _state.Test();
    _context.Set(_appRepo.Object);
    _context.Set(new JumpshroomLogic.Data(30));
  }

  [Fact]
  public void HitGoesToLoading()
  {
    var target = new Mock<IPushEnabled>();
    var next = _state.On(new JumpshroomLogicState.Input.Hit(target.Object));

    next.IsAssignableTo(typeof(JumpshroomLogicState.Loading)).ShouldBeTrue();
    _state.Get<JumpshroomLogic.Data>().Target.ShouldBe(target.Object);

    _appRepo.VerifyAll();
  }
}
