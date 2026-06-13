namespace GameDemo.Tests;

using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Chickensoft.Sync.Primitives;
using Moq;
using Shouldly;

[
  SuppressMessage(
    "Design",
    "CA1001",
    Justification = "Disposable fields are disposed in cleanup"
  )
]
public class PlayingTest : IDisposable
{
  private readonly StateTester _context;
  private readonly GameLogicState.Playing _state = new();
  private readonly Mock<IGameRepo> _gameRepo = new();
  private readonly AutoValue<bool> _isMouseCaptured = new(true);
  private readonly AutoValue<bool> _isPaused = new(true);

  public PlayingTest()
  {
    _context = _state.Test();

    _gameRepo.Setup(repo => repo.IsPaused).Returns(_isPaused);
    _gameRepo.Setup(repo => repo.IsMouseCaptured).Returns(_isMouseCaptured);

    _context.Set(_gameRepo.Object);
  }


  public void Dispose()
  {
    _isPaused.Dispose();
    _isMouseCaptured.Dispose();
  }

  [Fact]
  public void OnEnter()
  {
    _gameRepo.Reset();
    _gameRepo.Setup(repo => repo.SetIsMouseCaptured(true));
    _state.Enter();
    _context.Outputs.Single().ShouldBeOfType<GameLogicState.Output.StartGame>();
    _gameRepo.VerifyAll();
  }

  [Fact]
  public void OnPauseButtonPressed() =>
    _state.On(new GameLogicState.Input.PauseButtonPressed())
      .ShouldBe(typeof(GameLogicState.Paused));

  [Fact]
  public void OnEnded()
  {
    _state.OnEnded(GameOverReason.Won);
    _context.Inputs.Single().ShouldBeOfType<GameLogicState.Input.EndGame>();
  }

  [Fact]
  public void OnEndGameWins()
  {
    var result = _state.On(new GameLogicState.Input.EndGame(GameOverReason.Won));
    _gameRepo.Verify(repo => repo.Pause());
    result.ShouldBe(typeof(GameLogicState.Won));
  }

  [Fact]
  public void OnEndGameLoses()
  {
    var result = _state.On(new GameLogicState.Input.EndGame(GameOverReason.Lost));
    _gameRepo.Verify(repo => repo.Pause());
    result.ShouldBe(typeof(GameLogicState.Lost));
  }

  [Fact]
  public void OnEndGameQuits()
  {
    var result = _state.On(new GameLogicState.Input.EndGame(GameOverReason.Quit));
    _gameRepo.Verify(repo => repo.Pause());
    result.ShouldBe(typeof(GameLogicState.Quit));
  }

  [Fact]
  public void OnUnknownEndGameQuits()
  {
    var result = _state.On(new GameLogicState.Input.EndGame((GameOverReason)25));
    _gameRepo.Verify(repo => repo.Pause());
    result.ShouldBe(typeof(GameLogicState.Quit));
  }
}
