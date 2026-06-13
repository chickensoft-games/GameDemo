namespace GameDemo.Tests;

using Chickensoft.GoDotTest;
using Godot;
using Moq;
using Shouldly;

public class PlayerLogicStateDisabledTest : TestClass
{
  private StateTester _context = default!;
  private Mock<IAppRepo> _appRepo = default!;
  private PlayerLogicState.Disabled _state = default!;

  public PlayerLogicStateDisabledTest(Node testScene) : base(testScene) { }

  [Setup]
  public void Setup()
  {
    _appRepo = new ();
    _state = new();
    _context = _state.Test();

    _context.Set(_appRepo.Object);
  }

  [Test]
  public void EntersAndExits()
  {
    _state.Enter();

    _context.Outputs.ShouldBe([
      new PlayerLogicState.Output.Animations.Idle()
    ]);
  }

  [Test]
  public void IdlesOnEnable()
  {
    var next = _state.On(new PlayerLogicState.Input.Enable());

    next.IsAssignableTo(typeof(PlayerLogicState.Idle)).ShouldBeTrue();
  }

  [Test]
  public void OnGameAboutToStartEnables()
  {
    _state.OnGameEntered();

    _context.Inputs.ShouldBe([new PlayerLogicState.Input.Enable()]);
  }
}
