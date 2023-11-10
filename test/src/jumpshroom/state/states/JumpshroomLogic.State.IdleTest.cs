namespace GameDemo.Tests;

using Chickensoft.GoDotTest;
using Godot;
using Moq;
using Shouldly;

public class JumpshroomLogicStateIdleTest : TestClass {
  private JumpshroomLogic.IFakeContext _context = default!;
  private Mock<IAppRepo> _appRepo = default!;
  private JumpshroomLogic.State.Idle _state = default!;

  public JumpshroomLogicStateIdleTest(Node testScene) : base(testScene) { }

  [Setup]
  public void Setup() {
    _context = JumpshroomLogic.CreateFakeContext();
    _appRepo = new Mock<IAppRepo>();

    _context.Set(_appRepo.Object);

    _state = new(_context);
  }

  [Test]
  public void HitGoesToLoading() {
    var target = new Mock<IPushEnabled>();
    var next = _state.On(new JumpshroomLogic.Input.Hit(target.Object));

    var loading = next.ShouldBeOfType<JumpshroomLogic.State.Loading>();
    loading.Target.ShouldBe(target.Object);

    _appRepo.VerifyAll();
  }
}
