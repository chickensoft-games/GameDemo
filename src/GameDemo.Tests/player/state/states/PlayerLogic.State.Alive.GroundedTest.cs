namespace GameDemo.Tests;

using Chickensoft.Introspection;
using Chickensoft.LogicBlocks.Auto;
using Godot;
using Moq;
using Shouldly;

public partial class PlayerLogicStateAliveGroundedTest(GodotHeadlessFixture godot)
{
  [Meta, TestState]
  public partial record TestPlayerState : PlayerLogicState.Grounded;

  private StateTester _context = default!;
  private Mock<IPlayer> _player = default!;
  private Mock<IAppRepo> _appRepo = default!;
  private PlayerLogic.Settings _settings = default!;
  private PlayerLogicState.Grounded _state = default!;

  public PlayerLogicStateAliveGroundedTest(Node testScene) :
    base(testScene)
  { }

  [Setup]
  public void Setup()
  {
    _player = new Mock<IPlayer>();
    _appRepo = new Mock<IAppRepo>();
    _settings = new PlayerLogic.Settings(1, 1, 1, 1, 1, 1, 1);

    _state = new TestPlayerState();
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
