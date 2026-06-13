namespace GameDemo.Tests;

using Godot;
using Moq;
using Shouldly;

public class PlayerLogicStateAliveAirborneJumpingTest
{
  private readonly StateTester _context;
  private readonly Mock<IPlayer> _player = new();
  private readonly Mock<IAppRepo> _appRepo = new();
  private readonly Mock<IGameRepo> _gameRepo = new();
  private readonly PlayerLogic.Settings _settings = new(1, 1, 1, 1, 1, 1, JumpForce: 1);
  private readonly PlayerLogicState.Jumping _state = new();

  public PlayerLogicStateAliveAirborneJumpingTest()
  {
    _context = _state.Test();

    _context.Set(_player.Object);
    _context.Set(_appRepo.Object);
    _context.Set(_gameRepo.Object);
    _context.Set(_settings);
  }

  [Fact]
  public void Enters()
  {
    _gameRepo.Setup(repo => repo.OnJump());

    _state.Enter();

    _context.Outputs.ShouldBe([
      new PlayerLogicState.Output.Animations.Jump()
    ]);

    _gameRepo.VerifyAll();
  }

  [Fact]
  public void ContinuedJumpInputFurthersJump()
  {
    _player.Setup(player => player.Velocity).Returns(Vector3.Up);

    _state.On(new PlayerLogicState.Input.Jump(1));

    _context.Outputs.ShouldBe([
      new PlayerLogicState.Output.VelocityChanged(Vector3.Up + Vector3.Up)
    ]);
  }
}
