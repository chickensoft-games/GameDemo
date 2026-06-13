namespace GameDemo.Tests;

using Moq;
using Shouldly;

public class PlayerLogicStateDisabledTest
{
  private readonly StateTester _context;
  private readonly Mock<IAppRepo> _appRepo = new ();
  private readonly PlayerLogicState.Disabled _state = new();

  public PlayerLogicStateDisabledTest()
  {
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
