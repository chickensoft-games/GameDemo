namespace GameDemo.Tests;

using Chickensoft.GoDotTest;
using Chickensoft.LogicBlocks;
using Godot;
using Moq;
using Shouldly;

public class PlayerLogicStateDisabledTest : TestClass {
  private IFakeContext _context = default!;
  private Mock<IAppRepo> _appRepo = default!;
  private PlayerLogic.State.Disabled _state = default!;

  public PlayerLogicStateDisabledTest(Node testScene) : base(testScene) { }

  [Setup]
  public void Setup() {
    _appRepo = new();
    _state = new();
    _context = _state.CreateFakeContext();

    _context.Set(_appRepo.Object);
  }

  [Test]
  public void EntersAndExits() {
    _state.Enter();

    _context.Outputs.ShouldBe(new object[] {
      new PlayerLogic.Output.Animations.Idle()
    });
  }

  [Test]
  public void Subscribes() {
    _state.Attach(_context);
    _appRepo.VerifyAdd(
      (repo) => repo.GameStarting += _state.OnGameAboutToStart
    );

    _state.Detach();
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
