namespace GameDemo.Tests;

using System.Linq;
using Moq;
using Shouldly;

public class PausedSavingTest
{
  private readonly StateTester _context;
  private readonly GameLogicState.Saving _state = new();
  private readonly Mock<IGameRepo> _gameRepo = new();

  public PausedSavingTest()
  {
    _context = _state.Test();

    _context.Set(_gameRepo.Object);
  }

  [Fact]
  public void OnEnter()
  {
    _state.Enter(new GameLogicState.Paused());
    _context.Outputs.ShouldBeOfTypes(
      typeof(GameLogicState.Output.ShowPauseSaveOverlay),
      typeof(GameLogicState.Output.StartSaving)
    );
    _gameRepo.VerifyAll();
  }

  [Fact]
  public void OnExit()
  {
    _state.Exit(new GameLogicState.Paused());
    _context.Outputs.Single().ShouldBeOfType<GameLogicState.Output.HidePauseSaveOverlay>();
  }

  [Fact]
  public void OnInputSaveCompleted() =>
    _state.On(new GameLogicState.Input.SaveCompleted())
      .ShouldBe(typeof(GameLogicState.Paused));

  [Fact]
  public void OnPauseButtonPressed() =>
    _state.On(new GameLogicState.Input.PauseButtonPressed())
      .ShouldBe(_state.GetType());
}
