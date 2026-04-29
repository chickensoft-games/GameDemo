namespace GameDemo.Tests;

using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Chickensoft.GoDotTest;
using Godot;
using Shouldly;

[
  SuppressMessage(
    "Design",
    "CA1001",
    Justification = "Disposable fields are disposed in cleanup"
  )
]
public class PlayingTest : TestClass
{
  private StateTester _context = default!;
  private GameLogic.BaseState.Playing _state = default!;
  private IGameRepo _gameRepo = default!;

  public PlayingTest(Node testScene) : base(testScene) { }

  [Setup]
  public void Setup()
  {
    _state = new GameLogic.BaseState.Playing();
    _context = _state.Test();
    _gameRepo = new GameRepo();
    _context.Set(_gameRepo);
    GameLogic.SetupSubscriptions(_gameRepo, () => _state);
  }

  [Cleanup]
  public void Cleanup() => _gameRepo.Dispose();

  [Test]
  public void OnEnter()
  {
    _state.Enter();
    _context.Outputs.Single().ShouldBeOfType<GameLogic.Output.StartGame>();
    _gameRepo.IsMouseCaptured.Value.ShouldBe(true);
  }

  [Test]
  public void OnPauseButtonPressed() =>
    _state.On(new GameLogic.Input.PauseButtonPressed())
      .ShouldBe(typeof(GameLogic.BaseState.Paused));

  [Test]
  public void OnEnded()
  {
    _gameRepo.OnGameEnded(GameOverReason.Won);
    _context.Inputs.Single().ShouldBeOfType<GameLogic.Input.EndGame>();
  }

  [Test]
  public void OnEndGameWins()
  {
    var result = _state.On(new GameLogic.Input.EndGame(GameOverReason.Won));
    _gameRepo.IsPaused.Value.ShouldBe(true);
    result.ShouldBe(typeof(GameLogic.BaseState.Won));
  }

  [Test]
  public void OnEndGameLoses()
  {
    var result = _state.On(new GameLogic.Input.EndGame(GameOverReason.Lost));
    _gameRepo.IsPaused.Value.ShouldBe(true);
    result.ShouldBe(typeof(GameLogic.BaseState.Lost));
  }

  [Test]
  public void OnEndGameQuits()
  {
    var result = _state.On(new GameLogic.Input.EndGame(GameOverReason.Quit));
    _gameRepo.IsPaused.Value.ShouldBe(true);
    result.ShouldBe(typeof(GameLogic.BaseState.Quit));
  }

  [Test]
  public void OnUnknownEndGameQuits()
  {
    var result = _state.On(new GameLogic.Input.EndGame((GameOverReason)25));
    _gameRepo.IsPaused.Value.ShouldBe(true);
    result.ShouldBe(typeof(GameLogic.BaseState.Quit));
  }
}
