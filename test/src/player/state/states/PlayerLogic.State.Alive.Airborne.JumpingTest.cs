namespace GameDemo.Tests;

using Chickensoft.GoDotTest;
using Chickensoft.LogicBlocks;
using Godot;
using Moq;
using Shouldly;

public class PlayerLogicStateAliveAirborneJumpingTest : TestClass {
  private IFakeContext _context = default!;
  private Mock<IPlayer> _player = default!;
  private Mock<IAppRepo> _appRepo = default!;
  private PlayerLogic.Settings _settings = default!;
  private PlayerLogic.State.Jumping _state = default!;

  public PlayerLogicStateAliveAirborneJumpingTest(Node testScene) :
    base(testScene) { }

  [Setup]
  public void Setup() {
    _player = new Mock<IPlayer>();
    _appRepo = new Mock<IAppRepo>();
    _settings = new PlayerLogic.Settings(1, 1, 1, 1, 1, 1, JumpForce: 1);

    _state = new();
    _context = _state.CreateFakeContext();

    _context.Set(_player.Object);
    _context.Set(_appRepo.Object);
    _context.Set(_settings);
  }

  [Test]
  public void Enters() {
    var parent = new PlayerLogic.State.Airborne();

    _appRepo.Setup(repo => repo.Jump());

    _state.Enter(parent);

    _context.Outputs.ShouldBe(new object[] {
      new PlayerLogic.Output.Animations.Jump()
    });

    _appRepo.VerifyAll();
  }

  [Test]
  public void ContinuedJumpInputFurthersJump() {
    _player.Setup(player => player.Velocity).Returns(Vector3.Up);

    _state.On(new PlayerLogic.Input.Jump(1));

    _context.Outputs.ShouldBe(new object[] {
      new PlayerLogic.Output.VelocityChanged(Vector3.Up + Vector3.Up)
    });
  }
}
