namespace GameDemo.Tests;

using Chickensoft.Introspection;
using Chickensoft.LogicBlocks.Auto;
using Godot;
using Moq;
using Shouldly;

public partial class PlayerLogicStateAliveGroundedTest
{
  [Meta, TestState]
  public partial record TestPlayerState : PlayerLogicState.Grounded;

  private readonly StateTester _context;
  private readonly Mock<IPlayer> _player = new();
  private readonly Mock<IAppRepo> _appRepo = new();
  private readonly PlayerLogic.Settings _settings = new(1, 1, 1, 1, 1, 1, 1);
  private readonly PlayerLogicState.Grounded _state = new TestPlayerState();

  public PlayerLogicStateAliveGroundedTest()
  {
    _context = _state.Test();

    _context.Set(_player.Object);
    _context.Set(_appRepo.Object);
    _context.Set(_settings);
  }

  [Fact]
  public void JumpGoesToJumping()
  {
    _player.Setup(player => player.Velocity).Returns(Vector3.Zero);

    var next = _state.On(new PlayerLogicState.Input.Jump(1d));

    next.IsAssignableTo(typeof(PlayerLogicState.Jumping));

    _context.Outputs.ShouldBeOfTypes([
      typeof(PlayerLogicState.Output.VelocityChanged)
    ]);
  }

  [Fact]
  public void LeftFloorGoesToFallingOrLiftoff()
  {
    _state.On(new PlayerLogicState.Input.LeftFloor(IsFalling: true))
      .IsAssignableTo(typeof(PlayerLogicState.Falling));

    _state.On(new PlayerLogicState.Input.LeftFloor(IsFalling: false))
      .IsAssignableTo(typeof(PlayerLogicState.Liftoff));
  }
}
