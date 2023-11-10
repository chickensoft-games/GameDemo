namespace GameDemo.Tests;

using Chickensoft.GoDotTest;
using Godot;
using Moq;
using Shouldly;

public class CoinLogicStateCollectingTest : TestClass {
  private CoinLogic.IFakeContext _context = default!;
  private Mock<IAppRepo> _appRepo = default!;
  private CoinLogic.Settings _settings = default!;
  private Mock<ICoin> _coin = default!;
  private Mock<ICoinCollector> _target = default!;
  private CoinLogic.State.Collecting _state = default!;

  public CoinLogicStateCollectingTest(Node testScene) : base(testScene) { }

  [Setup]
  public void Setup() {
    _context = CoinLogic.CreateFakeContext();

    _appRepo = new();
    _settings = new(1.0f);
    _coin = new();
    _target = new();

    _context.Set(_settings);
    _context.Set(_appRepo.Object);
    _context.Set(_coin.Object);

    _state = new(_context, _target.Object);
  }

  [Test]
  public void Enters() {
    _appRepo.Setup(repo => repo.StartCoinCollection(_coin.Object));

    _state.Enter();

    _appRepo.VerifyAll();
  }

  [Test]
  public void ComputesNextPositionOnPhysicsProcess() {
    var input = new CoinLogic.Input.PhysicsProcess(
      1.0f,
      GlobalPosition: Vector3.Zero
    );

    _appRepo.Setup(repo => repo.OnFinishCoinCollection(_coin.Object));
    _target.Setup(target => target.CenterOfMass).Returns(Vector3.One);

    _state.On(input).ShouldBe(_state);

    _context.Outputs.ShouldBeOfTypes(new System.Type[] {
      typeof(CoinLogic.Output.SelfDestruct),
      typeof(CoinLogic.Output.Move),
    });
  }
}
