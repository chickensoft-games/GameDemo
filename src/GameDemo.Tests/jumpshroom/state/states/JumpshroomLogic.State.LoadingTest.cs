namespace GameDemo.Tests;

using Moq;
using Shouldly;

public class JumpshroomLogicStateLoadingTest
{
  private readonly StateTester _context;
  private readonly Mock<IPushEnabled> _target = new();
  private readonly Mock<IGameRepo> _gameRepo = new();
  private readonly JumpshroomLogic.Data _data;
  private readonly JumpshroomLogicState.Loading _state = new();

  public JumpshroomLogicStateLoadingTest()
  {
    _data = new(1.0f)
    {
      Target = _target.Object
    };

    _context = _state.Test();

    _context.Set(_gameRepo.Object);
    _context.Set(_data);
  }

  [Fact]
  public void AnimatesAndAnnouncesJumpshroomUseOnEnter()
  {
    _gameRepo.Setup(repo => repo.OnJumpshroomUsed());

    _state.Enter();

    _context.Outputs.ShouldBe([
      new JumpshroomLogicState.Output.Animate()
    ]);

    _gameRepo.VerifyAll();
  }

  [Fact]
  public void LaunchGoesToLaunching()
  {
    var next = _state.On(new JumpshroomLogicState.Input.Launch());

    next.IsAssignableTo(typeof(JumpshroomLogicState.Launching)).ShouldBeTrue();
  }
}
