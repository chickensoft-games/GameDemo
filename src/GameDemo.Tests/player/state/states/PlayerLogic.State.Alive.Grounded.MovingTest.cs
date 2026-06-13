namespace GameDemo.Tests;

using Moq;
using Shouldly;

public class PlayerLogicStateAliveGroundedMovingTest
{
  private readonly StateTester _context;
  private readonly Mock<IAppRepo> _appRepo = new();
  private readonly PlayerLogicState.Moving _state = new();

  public PlayerLogicStateAliveGroundedMovingTest()
  {
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
