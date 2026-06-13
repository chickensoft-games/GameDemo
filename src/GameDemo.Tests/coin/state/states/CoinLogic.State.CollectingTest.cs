namespace GameDemo.Tests;

using Chickensoft.Collections;
using Godot;
using Moq;
using Shouldly;

public class CoinLogicStateCollectingTest
{
  private readonly StateTester _context;
  private readonly Mock<IGameRepo> _gameRepo = new();
  private readonly CoinLogic.Settings _settings = new(1.0f);
  private readonly Mock<ICoin> _coin = new();
  private readonly Mock<ICoinCollector> _target = new();
  private readonly CoinLogicState.Collecting _state = new();
  private readonly CoinLogic.Data _data = new() { Target = "target_id" };
  private readonly EntityTable _entityTable = new();

  public CoinLogicStateCollectingTest()
  {
    _context = _state.Test();

    _entityTable.Set("target_id", _target.Object);

    _context.Set(_gameRepo.Object);
    _context.Set(_settings);
    _context.Set(_coin.Object);
    _context.Set(_target.Object);
    _context.Set(_data);
    _context.Set(_entityTable);
  }

  [Fact]
  public void Enters()
  {
    _gameRepo.Setup(repo => repo.StartCoinCollection(_coin.Object));

    _state.Enter();

    _gameRepo.VerifyAll();
  }

  [Fact]
  public void ComputesNextPositionOnPhysicsProcess()
  {
    var input = new CoinLogicState.Input.PhysicsProcess(
      1.0f,
      GlobalPosition: Vector3.Zero
    );

    _gameRepo.Setup(repo => repo.OnFinishCoinCollection(_coin.Object));
    _target.Setup(target => target.CenterOfMass).Returns(Vector3.One);

    _state.On(input).ShouldBe(_state.GetType());

    _context.Outputs.ShouldBeOfTypes(
      typeof(CoinLogicState.Output.SelfDestruct),
      typeof(CoinLogicState.Output.Move)
    );
  }
}
