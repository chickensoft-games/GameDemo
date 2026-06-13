namespace GameDemo.Tests;

using Godot;
using Moq;
using Shouldly;

public class PlayerLogicStateAliveGroundedMovingTest(GodotHeadlessFixture godot)
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

  [Fact]
  public void Enters()
  {
    _state.Enter(new PlayerLogicState.Idle());

    _context.Outputs.ShouldBe([
      new PlayerLogicState.Output.Animations.Move()
    ]);
  }

  [Fact]
  public void OnStoppedMovingHorizontallyIdles()
  {
    var next = _state.On(new PlayerLogicState.Input.StoppedMovingHorizontally());

    next.IsAssignableTo(typeof(PlayerLogicState.Idle));
  }
}
