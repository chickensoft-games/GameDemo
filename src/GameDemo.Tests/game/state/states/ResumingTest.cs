namespace GameDemo.Tests;

using System.Linq;
using Godot;
using Moq;
using Shouldly;

public class ResumingTest(GodotHeadlessFixture godot)
{
  private StateTester _context = default!;
  private GameLogicState.Resuming _state = default!;
  private Mock<IGameRepo> _gameRepo = default!;

  public ResumingTest(Node testScene) : base(testScene) { }

  [Setup]
  public void Setup()
  {
    _state = new GameLogicState.Resuming();
    _context = _state.Test();

    _gameRepo = new();
    _context.Set(_gameRepo.Object);
  }

  [Fact]
  public void OnEnter()
  {
    _gameRepo.Setup(repo => repo.Resume());
    _state.Enter();
    _gameRepo.VerifyAll();
  }

  [Fact]
  public void OnExit()
  {
    _state.Exit();
    _context.Outputs.Single().ShouldBeOfType<GameLogicState.Output.HidePauseMenu>();
  }

  [Fact]
  public void OnPauseMenuTransitioned()
  {
    var result = _state.On(new GameLogicState.Input.PauseMenuTransitioned());
    result.ShouldBe(typeof(GameLogicState.Playing));
  }
}
