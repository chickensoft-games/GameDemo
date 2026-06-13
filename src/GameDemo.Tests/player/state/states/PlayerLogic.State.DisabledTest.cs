namespace GameDemo.Tests;

using Godot;
using Moq;
using Shouldly;

public class PlayerLogicStateDisabledTest(GodotHeadlessFixture godot)
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

  [Fact]
  public void EntersAndExits()
  {
    _state.Enter();

    _context.Outputs.ShouldBe([
      new PlayerLogicState.Output.Animations.Idle()
    ]);
  }

  [Fact]
  public void IdlesOnEnable()
  {
    var next = _state.On(new PlayerLogicState.Input.Enable());

    next.IsAssignableTo(typeof(PlayerLogicState.Idle)).ShouldBeTrue();
  }

  [Fact]
  public void OnGameAboutToStartEnables()
  {
    _state.OnGameEntered();

    _context.Inputs.ShouldBe([new PlayerLogicState.Input.Enable()]);
  }
}
