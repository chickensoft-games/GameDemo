namespace GameDemo.Tests;

using Chickensoft.GoDotTest;
using Chickensoft.LogicBlocks;
using Godot;
using Moq;
using Shouldly;

public class CoinLogicStateCollectingTest : TestClass {
  private IFakeContext _context = default!;
  private Mock<IGameRepo> _gameRepo = default!;
  private CoinLogic.Settings _settings = default!;
  private Mock<ICoin> _coin = default!;
  private Mock<ICoinCollector> _target = default!;
  private CoinLogic.State.Collecting _state = default!;

  public CoinLogicStateCollectingTest(Node testScene) : base(testScene) { }

  [Setup]
  public void Setup() {
    _gameRepo = new();
    _settings = new(1.0f);
    _coin = new();
    _target = new();

    _state = new(_target.Object);
    _context = _state.CreateFakeContext();

    _context.Set(_settings);
    _context.Set(_gameRepo.Object);
    _context.Set(_coin.Object);
  }

  [Test]
  public void Enters() {
    _gameRepo.Setup(repo => repo.StartCoinCollection(_coin.Object));

    _state.Enter();

    _gameRepo.VerifyAll();
  }

  [Test]
  public void ComputesNextPositionOnPhysicsProcess() {
    var input = new CoinLogic.Input.PhysicsProcess(
      1.0f,
      GlobalPosition: Vector3.Zero
    );

    _gameRepo.Setup(repo => repo.OnFinishCoinCollection(_coin.Object));
    _target.Setup(target => target.CenterOfMass).Returns(Vector3.One);

    _state.On(input).ShouldBe(_state);

    _context.Outputs.ShouldBeOfTypes(typeof(CoinLogic.Output.SelfDestruct),
      typeof(CoinLogic.Output.Move));
  }
}
