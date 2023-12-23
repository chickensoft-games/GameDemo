namespace GameDemo.Tests;

using Chickensoft.GoDotTest;
using Chickensoft.LogicBlocks;
using Godot;
using Moq;
using Shouldly;

public class JumpshroomLogicStateLoadingTest : TestClass {
  private IFakeContext _context = default!;
  private Mock<IPushEnabled> _target = default!;
  private Mock<IAppRepo> _appRepo = default!;
  private JumpshroomLogic.Data _data = default!;
  private JumpshroomLogic.State.Loading _state = default!;

  public JumpshroomLogicStateLoadingTest(Node testScene) : base(testScene) { }

  [Setup]
  public void Setup() {
    _target = new Mock<IPushEnabled>();
    _appRepo = new Mock<IAppRepo>();
    _data = new(1.0f);

    _state = new(_target.Object);
    _context = _state.CreateFakeContext();

    _context.Set(_appRepo.Object);
    _context.Set(_data);
  }

  [Test]
  public void AnimatesAndAnnouncesJumpshroomUseOnEnter() {
    _appRepo.Setup(repo => repo.OnJumpshroomUsed());

    _state.Enter();

    _context.Outputs.ShouldBe(new object[] {
      new JumpshroomLogic.Output.Animate(),
    });

    _appRepo.VerifyAll();
  }

  [Test]
  public void LaunchGoesToLaunching() {
    var next = _state.On(new JumpshroomLogic.Input.Launch());

    next.ShouldBeOfType<JumpshroomLogic.State.Launching>();
  }
}
