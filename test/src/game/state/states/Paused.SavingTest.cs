namespace GameDemo.Tests;

using System.Linq;
using Chickensoft.GoDotTest;
using Chickensoft.LogicBlocks;
using Godot;
using Moq;
using Shouldly;

public class PausedSavingTest : TestClass {
  private IFakeContext _context = default!;
  private GameLogic.State.Saving _state = default!;
  private Mock<IGameRepo> _gameRepo = default!;

  public PausedSavingTest(Node testScene) : base(testScene) { }

  [Setup]
  public void Setup() {
    _state = new();
    _context = _state.CreateFakeContext();

    _gameRepo = new();

    _context.Set(_gameRepo.Object);
  }

  [Test]
  public void OnEnter() {
    _state.Enter(new GameLogic.State.Paused());
    _context.Outputs.ShouldBeOfTypes(
      typeof(GameLogic.Output.ShowPauseSaveOverlay),
      typeof(GameLogic.Output.StartSaving)
    );
    _gameRepo.VerifyAll();
  }

  [Test]
  public void OnExit() {
    _state.Exit(new GameLogic.State.Paused());
    _context.Outputs.Single().ShouldBeOfType<GameLogic.Output.HidePauseSaveOverlay>();
  }

  [Test]
  public void OnInputSaveCompleted() =>
    _state.On(new GameLogic.Input.SaveCompleted()).State
      .ShouldBeOfType<GameLogic.State.Paused>();

  [Test]
  public void OnPauseButtonPressed() =>
    _state.On(new GameLogic.Input.PauseButtonPressed()).State
      .ShouldBe(_state);
}
