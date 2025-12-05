namespace GameDemo.Tests;

using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Chickensoft.GoDotTest;
using Chickensoft.LogicBlocks;
using Chickensoft.Sync.Primitives;
using Godot;
using Moq;
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
  private IFakeContext _context = default!;
  private GameLogic.State.Playing _state = default!;
  private Mock<IGameRepo> _gameRepo = default!;
  private AutoValue<bool> _isMouseCaptured = default!;
  private AutoValue<bool> _isPaused = default!;

  public PlayingTest(Node testScene) : base(testScene) { }

  [Setup]
  public void Setup()
  {
    _state = new GameLogic.State.Playing();
    _context = _state.CreateFakeContext();

    _isMouseCaptured = new(true);
    _isPaused = new(true);

    _gameRepo = new();
    _gameRepo.Setup(repo => repo.IsPaused).Returns(_isPaused);
    _gameRepo.Setup(repo => repo.IsMouseCaptured).Returns(_isMouseCaptured);

    _context.Set(_gameRepo.Object);
  }

  [Cleanup]
  public void Cleanup()
  {
    _isPaused.Dispose();
    _isMouseCaptured.Dispose();
  }

  [Test]
  public void OnEnter()
  {
    _gameRepo.Reset();
    _gameRepo.Setup(repo => repo.SetIsMouseCaptured(true));
    _state.Enter();
    _context.Outputs.Single().ShouldBeOfType<GameLogic.Output.StartGame>();
    _gameRepo.VerifyAll();
  }

  [Test]
  public void Subscribes()
  {
    _state.Attach(_context);

    _gameRepo.VerifyAdd(repo => repo.Ended += _state.OnEnded);

    _state.Detach();

    _gameRepo.VerifyRemove(repo => repo.Ended -= _state.OnEnded);
  }

  [Test]
  public void OnPauseButtonPressed() =>
    _state.On(new GameLogic.Input.PauseButtonPressed()).State
      .ShouldBeOfType<GameLogic.State.Paused>();

  [Test]
  public void OnEnded()
  {
    _state.OnEnded(GameOverReason.Won);
    _context.Inputs.Single().ShouldBeOfType<GameLogic.Input.EndGame>();
  }

  [Test]
  public void OnEndGameWins()
  {
    var result = _state.On(new GameLogic.Input.EndGame(GameOverReason.Won));
    _gameRepo.Verify(repo => repo.Pause());
    result.State.ShouldBeOfType<GameLogic.State.Won>();
  }

  [Test]
  public void OnEndGameLoses()
  {
    var result = _state.On(new GameLogic.Input.EndGame(GameOverReason.Lost));
    _gameRepo.Verify(repo => repo.Pause());
    result.State.ShouldBeOfType<GameLogic.State.Lost>();
  }

  [Test]
  public void OnEndGameQuits()
  {
    var result = _state.On(new GameLogic.Input.EndGame(GameOverReason.Quit));
    _gameRepo.Verify(repo => repo.Pause());
    result.State.ShouldBeOfType<GameLogic.State.Quit>();
  }

  [Test]
  public void OnUnknownEndGameQuits()
  {
    var result = _state.On(new GameLogic.Input.EndGame((GameOverReason)25));
    _gameRepo.Verify(repo => repo.Pause());
    result.State.ShouldBeOfType<GameLogic.State.Quit>();
  }
}
