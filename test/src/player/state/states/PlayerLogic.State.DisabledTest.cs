namespace GameDemo.Tests;

using Chickensoft.GoDotTest;
using Chickensoft.LogicBlocks;
using Godot;
using Moq;
using Shouldly;

public class PlayerLogicStateDisabledTest : TestClass
{
  private StateTester _context = default!;
  private Mock<IAppRepo> _appRepo = default!;
  private PlayerLogic.BaseState.Disabled _state = default!;

  public PlayerLogicStateDisabledTest(Node testScene) : base(testScene) { }

  [Setup]
  public void Setup()
  {
    _appRepo = new();
    _state = new();
    _context = _state.Test();

    _context.Set(_appRepo.Object);
  }

  [Test]
  public void EntersAndExits()
  {
    _state.Enter();

    _context.Outputs.ShouldBe([
      new PlayerLogic.Output.Animations.Idle()
    ]);
  }

  [Test]
  public void IdlesOnEnable()
  {
    var next = _state.On(new PlayerLogic.Input.Enable());

    next.ShouldBeAssignableTo<PlayerLogic.BaseState.Idle>();
  }

  [Test]
  public void OnGameAboutToStartEnables()
  {
    _state.Input(new PlayerLogic.Input.Enable());

    _context.Inputs.ShouldBe([new PlayerLogic.Input.Enable()]);
  }
}
