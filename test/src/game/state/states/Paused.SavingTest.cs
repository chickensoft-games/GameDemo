namespace GameDemo.Tests;

using System.Linq;
using Chickensoft.GoDotTest;
using Chickensoft.LogicBlocks;
using Godot;
using Moq;
using Shouldly;

public class PausedSavingTest : TestClass
{
  private StateTester _context = default!;
  private GameLogicState.Saving _state = default!;
  private Mock<IGameRepo> _gameRepo = default!;

  public PausedSavingTest(Node testScene) : base(testScene) { }

  [Setup]
  public void Setup()
  {
    _state = new();
    _context = _state.Test();

    _gameRepo = new();

    _context.Set(_gameRepo.Object);
  }

  [Test]
  public void OnEnter()
  {
    _state.Enter(new GameLogicState.Paused());
    _context.Outputs.ShouldBeOfTypes(
      typeof(GameLogicState.Output.ShowPauseSaveOverlay),
      typeof(GameLogicState.Output.StartSaving)
    );
    _gameRepo.VerifyAll();
  }

  [Test]
  public void OnExit()
  {
    _state.Exit(new GameLogicState.Paused());
    _context.Outputs.Single().ShouldBeOfType<GameLogicState.Output.HidePauseSaveOverlay>();
  }

  [Test]
  public void OnInputSaveCompleted() =>
    _state.On(new GameLogicState.Input.SaveCompleted())
      .ShouldBe(typeof(GameLogicState.Paused));

  [Test]
  public void OnPauseButtonPressed() =>
    _state.On(new GameLogicState.Input.PauseButtonPressed())
      .ShouldBe(_state.GetType());
}
