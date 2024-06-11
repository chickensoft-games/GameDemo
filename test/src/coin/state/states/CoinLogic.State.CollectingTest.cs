namespace GameDemo.Tests;

using Chickensoft.Collections;
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
  private CoinLogic.Data _data = default!;
  private EntityTable _entityTable = default!;

  public CoinLogicStateCollectingTest(Node testScene) : base(testScene) { }

  [Setup]
  public void Setup() {
    _state = new();
    _gameRepo = new();
    _settings = new(1.0f);
    _coin = new();
    _target = new();
    _data = new() { Target = "target_id" };
    _entityTable = new();

    _context = _state.CreateFakeContext();

    _entityTable.Set("target_id", _target.Object);

    _context.Set(_gameRepo.Object);
    _context.Set(_settings);
    _context.Set(_coin.Object);
    _context.Set(_target.Object);
    _context.Set(_data);
    _context.Set(_entityTable);
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

    _state.On(input).State.ShouldBe(_state);

    _context.Outputs.ShouldBeOfTypes(
      typeof(CoinLogic.Output.SelfDestruct),
      typeof(CoinLogic.Output.Move)
    );
  }
}
