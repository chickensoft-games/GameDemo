namespace GameDemo.Tests;

using Godot;
using Moq;
using Shouldly;

public class JumpshroomLogicStateLoadingTest(GodotHeadlessFixture godot)
{
  private StateTester _context = default!;
  private Mock<IPushEnabled> _target = default!;
  private Mock<IGameRepo> _gameRepo = default!;
  private JumpshroomLogic.Data _data = default!;
  private JumpshroomLogicState.Loading _state = default!;

  public JumpshroomLogicStateLoadingTest(Node testScene) : base(testScene) { }

  [Setup]
  public void Setup()
  {
    _target = new();
    _gameRepo = new();
    _data = new(1.0f)
    {
      Target = _target.Object
    };

    _state = new();
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
