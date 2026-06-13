namespace GameDemo.Tests;

using Godot;
using Moq;
using Shouldly;

public class JumpshroomLogicStateLaunchingTest
{
  private readonly StateTester _context;
  private readonly Mock<IPushEnabled> _target = new();
  private readonly JumpshroomLogic.Data _data;
  private readonly JumpshroomLogicState.Launching _state = new();

  public JumpshroomLogicStateLaunchingTest()
  {
    _data = new(1.0f)
    {
      Target = _target.Object
    };

    _context = _state.Test();

    _context.Set(_data);
  }

  [Fact]
  public void Enters()
  {
    _target.Setup(t => t.Push(It.IsAny<Vector3>()));

    _state.Enter();

    _target.VerifyAll();
  }

  [Fact]
  public void LaunchCompletedGoesToCooldown()
  {
    var next = _state.On(new JumpshroomLogicState.Input.LaunchCompleted());

    next.IsAssignableTo(typeof(JumpshroomLogicState.Cooldown)).ShouldBeTrue();
  }
}
