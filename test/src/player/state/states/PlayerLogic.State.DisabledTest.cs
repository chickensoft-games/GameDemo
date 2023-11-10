namespace GameDemo.Tests;

using Chickensoft.GoDotTest;
using Godot;
using Moq;
using Shouldly;

public class PlayerLogicStateDisabledTest : TestClass {
  private PlayerLogic.IFakeContext _context = default!;
  private Mock<IAppRepo> _appRepo = default!;
  private PlayerLogic.State.Disabled _state = default!;

  public PlayerLogicStateDisabledTest(Node testScene) : base(testScene) { }

  [Setup]
  public void Setup() {
    _context = PlayerLogic.CreateFakeContext();
    _appRepo = new();

    _context.Set(_appRepo.Object);
    _state = new(_context);
  }

  [Test]
  public void EntersAndExits() {
    _state.Enter();

    _appRepo.VerifyAdd(
      (repo) => repo.GameStarting += _state.OnGameAboutToStart
    );

    _context.Outputs.ShouldBe(new object[] {
      new PlayerLogic.Output.Animations.Idle()
    });

    _state.Exit();

    _appRepo.VerifyRemove(
      (repo) => repo.GameStarting -= _state.OnGameAboutToStart
    );
  }

  [Test]
  public void IdlesOnEnable() {
    var next = _state.On(new PlayerLogic.Input.Enable());

    next.ShouldBeAssignableTo<PlayerLogic.State.Idle>();
  }

  [Test]
  public void OnGameAboutToStartEnables() {
    _state.OnGameAboutToStart();

    _context.Inputs.ShouldBe(new object[] {
      new PlayerLogic.Input.Enable()
    });
  }
}
