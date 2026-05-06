namespace GameDemo.Tests;

using Chickensoft.GoDotTest;
using Chickensoft.LogicBlocks;
using Godot;
using Moq;
using Shouldly;

public class JumpshroomLogicStateIdleTest : TestClass
{
  private StateTester _context = default!;
  private Mock<IAppRepo> _appRepo = default!;
  private JumpshroomLogic.BaseState.Idle _state = default!;

  public JumpshroomLogicStateIdleTest(Node testScene) : base(testScene) { }

  [Setup]
  public void Setup()
  {
    _appRepo = new Mock<IAppRepo>();

    _state = new();
    _context = _state.Test();
    _context.Set(_appRepo.Object);
    _context.Set(new JumpshroomLogic.Data(30));
  }

  [Test]
  public void HitGoesToLoading()
  {
    var target = new Mock<IPushEnabled>();
    var next = _state.On(new JumpshroomLogic.Input.Hit(target.Object));

    next.IsAssignableTo(typeof(JumpshroomLogic.BaseState.Loading)).ShouldBeTrue();
    _state.Get<JumpshroomLogic.Data>().Target.ShouldBe(target.Object);

    _appRepo.VerifyAll();
  }
}
