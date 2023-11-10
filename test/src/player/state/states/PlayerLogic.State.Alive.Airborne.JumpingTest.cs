namespace GameDemo.Tests;

using Chickensoft.GoDotTest;
using Godot;
using Moq;
using Shouldly;

public class PlayerLogicStateAliveAirborneJumpingTest : TestClass {
  private PlayerLogic.IFakeContext _context = default!;
  private Mock<IPlayer> _player = default!;
  private Mock<IAppRepo> _appRepo = default!;
  private PlayerLogic.Settings _settings = default!;
  private PlayerLogic.State.Jumping _state = default!;

  public PlayerLogicStateAliveAirborneJumpingTest(Node testScene) :
    base(testScene) { }

  [Setup]
  public void Setup() {
    _context = PlayerLogic.CreateFakeContext();

    _player = new Mock<IPlayer>();
    _appRepo = new Mock<IAppRepo>();
    _settings = new PlayerLogic.Settings(1, 1, 1, 1, 1, 1, JumpForce: 1);

    _context.Set(_player.Object);
    _context.Set(_appRepo.Object);
    _context.Set(_settings);

    _state = new(_context);
  }

  [Test]
  public void Enters() {
    var parent =
      new PlayerLogic.State.Airborne(new Mock<PlayerLogic.IContext>().Object);

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
