namespace GameDemo.Tests;

using Chickensoft.GoDotTest;
using Godot;
using Moq;
using Shouldly;

public class JumpshroomLogicStateLaunchingTest : TestClass {
  private JumpshroomLogic.IFakeContext _context = default!;
  private Mock<IPushEnabled> _target = default!;
  private JumpshroomLogic.Data _data = default!;
  private JumpshroomLogic.State.Launching _state = default!;

  public JumpshroomLogicStateLaunchingTest(Node testScene) : base(testScene) { }

  [Setup]
  public void Setup() {
    _context = JumpshroomLogic.CreateFakeContext();

    _target = new Mock<IPushEnabled>();
    _data = new(1.0f);

    _context.Set(_data);

    _state = new(_context, _target.Object);
  }

  [Test]
  public void Enters() {
    _target.Setup(t => t.Push(It.IsAny<Vector3>()));

    _state.Enter();

    _target.VerifyAll();
  }

  [Test]
  public void LaunchCompletedGoesToCooldown() {
    var next = _state.On(new JumpshroomLogic.Input.LaunchCompleted());

    next.ShouldBeOfType<JumpshroomLogic.State.Cooldown>();
  }
}
