namespace GameDemo.Tests;

using Chickensoft.GoDotTest;
using Chickensoft.LogicBlocks;
using Godot;
using Moq;
using Shouldly;

public class JumpshroomLogicStateIdleTest : TestClass {
  private IFakeContext _context = default!;
  private Mock<IAppRepo> _appRepo = default!;
  private JumpshroomLogic.State.Idle _state = default!;

  public JumpshroomLogicStateIdleTest(Node testScene) : base(testScene) { }

  [Setup]
  public void Setup() {
    _appRepo = new Mock<IAppRepo>();

    _state = new();
    _context = _state.CreateFakeContext();
    _context.Set(_appRepo.Object);
  }

  [Test]
  public void HitGoesToLoading() {
    var target = new Mock<IPushEnabled>();
    var next = _state.On(new JumpshroomLogic.Input.Hit(target.Object));

    var loading = next.State.ShouldBeOfType<JumpshroomLogic.State.Loading>();
    loading.Target.ShouldBe(target.Object);

    _appRepo.VerifyAll();
  }
}
