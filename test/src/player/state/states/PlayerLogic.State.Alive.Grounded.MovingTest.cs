namespace GameDemo.Tests;

using Chickensoft.GoDotTest;
using Chickensoft.LogicBlocks;
using Godot;
using Moq;
using Shouldly;

public class PlayerLogicStateAliveGroundedMovingTest : TestClass
{
  private StateTester _context = default!;
  private Mock<IAppRepo> _appRepo = default!;
  private PlayerLogicState.Moving _state = default!;

  public PlayerLogicStateAliveGroundedMovingTest(Node testScene) :
    base(testScene)
  { }

  [Setup]
  public void Setup()
  {
    _appRepo = new();
    _state = new();
    _context = _state.Test();

    _context.Set(_appRepo.Object);
  }

  [Test]
  public void Enters()
  {
    _state.Enter(new PlayerLogicState.Idle());

    _context.Outputs.ShouldBe([
      new PlayerLogicState.Output.Animations.Move()
    ]);
  }

  [Test]
  public void OnStoppedMovingHorizontallyIdles()
  {
    var next = _state.On(new PlayerLogicState.Input.StoppedMovingHorizontally());

    next.IsAssignableTo(typeof(PlayerLogicState.Idle));
  }
}
