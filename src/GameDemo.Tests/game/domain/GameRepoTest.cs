namespace GameDemo.Tests;

using System.Diagnostics.CodeAnalysis;
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
public class GameRepoTest : IDisposable
{
  private readonly AutoValue<bool> _isMouseCaptured = new(false);
  private readonly AutoValue<bool> _isPaused = new(false);
  private readonly AutoValue<Vector3> _playerGlobalPosition = new(Vector3.Zero);
  private readonly AutoValue<Basis> _cameraBasis = new(Basis.Identity);
  private readonly AutoValue<int> _numCoinsCollected = new(0);
  private readonly AutoValue<int> _numCoinsAtStart = new(0);

  private readonly GameRepo _repo;

  public GameRepoTest()
  {
    _repo = new(
      _isMouseCaptured,
      _isPaused,
      _playerGlobalPosition,
      _cameraBasis,
      _numCoinsCollected,
      _numCoinsAtStart
    );
  }

  public void Dispose()
  {
    _repo.Dispose();
    _numCoinsAtStart.Dispose();
    _numCoinsCollected.Dispose();
    _cameraBasis.Dispose();
    _playerGlobalPosition.Dispose();
    _isPaused.Dispose();
    _isMouseCaptured.Dispose();
    GC.SuppressFinalize(this);
  }

  [Fact]
  public void Initializes()
  {
    var repo = new GameRepo();
    repo.ShouldBeAssignableTo<IGameRepo>();
  }

  [Fact]
  public void SetPlayerGlobalPosition()
  {
    _repo.SetPlayerGlobalPosition(Vector3.One);

    _repo.PlayerGlobalPosition.Value.ShouldBe(Vector3.One);
  }

  [Fact]
  public void SetIsMouseCaptured()
  {
    _repo.SetIsMouseCaptured(true);

    _repo.IsMouseCaptured.Value.ShouldBe(true);
  }

  [Fact]
  public void SetCameraBasis()
  {
    _repo.SetCameraBasis(Basis.Identity);

    _repo.CameraBasis.Value.ShouldBe(Basis.Identity);
  }

  [Fact]
  public void StartCoinCollection()
  {
    var coin = new Mock<ICoin>();
    var called = false;

    _repo.AutoChannel.Bind().On((in IGameRepo.CoinCollectionStarted _) => called = true);

    _repo.StartCoinCollection(coin.Object);

    _numCoinsCollected.Value.ShouldBe(1);

    called.ShouldBeTrue();
  }

  [Fact]
  public void StartCoinCollectionDoesNotInvokeEventIfNoListeners()
  {
    var coin = new Mock<ICoin>();

    _repo.StartCoinCollection(coin.Object);

    _numCoinsCollected.Value.ShouldBe(1);
  }

  [Fact]
  public void OnFinishCoinCollectionTriggersWin()
  {
    var coin = new Mock<ICoin>();
    var coins = 0;
    GameOverReason gameOverReason = default!;

    var binding = _repo.AutoChannel.Bind();
    binding.On((in IGameRepo.Ended message) => gameOverReason = message.Reason);
    binding.On((in IGameRepo.CoinCollectionStarted _) => coins++);
    _repo.StartCoinCollection(coin.Object);

    _repo.NumCoinsCollected.Value.ShouldBe(1);
    coins.ShouldBe(1);

    _repo.OnFinishCoinCollection(coin.Object);

    gameOverReason.ShouldBe(GameOverReason.Won);

    binding.Dispose();
  }

  [Fact]
  public void OnFinishCoinCollectionDoesNothingIfNonZeroAmountOfCoins()
  {
    var coin = new Mock<ICoin>();
    Should.NotThrow(() => _repo.OnFinishCoinCollection(coin.Object));
  }

  [Fact]
  public void OnFinishCoinCollectionInvokes()
  {
    var called = false;

    var coin = new Mock<ICoin>();

    var binding = _repo.AutoChannel.Bind();
    binding.On((in IGameRepo.CoinCollectionCompleted _) => called = true);

    _repo.OnFinishCoinCollection(coin.Object);

    binding.Dispose();

    called.ShouldBeTrue();
  }

  [Fact]
  public void OnJumpInvokesEvent()
  {
    Should.NotThrow(_repo.OnJump);

    var called = false;
    _repo.AutoChannel.Bind().On((in IGameRepo.Jumped _) => called = true);

    _repo.OnJump();

    called.ShouldBeTrue();
  }

  [Fact]
  public void OnGameEndedPausesAndInvokesGameEnded()
  {
    var called = false;
    _repo.AutoChannel.Bind().On((in IGameRepo.Ended _) => called = true);

    _repo.OnGameEnded(GameOverReason.Won);

    called.ShouldBeTrue();
    _repo.IsPaused.Value.ShouldBeTrue();
    _repo.IsMouseCaptured.Value.ShouldBe(false);
  }

  [Fact]
  public void OnGameEndedDoesNotInvokeEventIfNoListeners() =>
    Should.NotThrow(() => _repo.OnGameEnded(GameOverReason.Won));

  [Fact]
  public void Pause()
  {
    _repo.Pause();
    _isMouseCaptured.Value.ShouldBe(false);
    _isPaused.Value.ShouldBe(true);
  }

  [Fact]
  public void Resume()
  {
    _repo.Resume();
    _isMouseCaptured.Value.ShouldBe(true);
    _isPaused.Value.ShouldBe(false);
  }

  [Fact]
  public void OnJumpshroomUsedInvokesEvent()
  {
    var called = 0;

    _repo.OnJumpshroomUsed();
    _repo.AutoChannel.Bind().On((in IGameRepo.JumpshroomUsed _) => called++);
    _repo.OnJumpshroomUsed();

    called.ShouldBe(1);
  }

  [Fact]
  public void SetNumCoinsAtStart()
  {
    _repo.SetNumCoinsAtStart(5);

    _repo.NumCoinsAtStart.Value.ShouldBe(5);
  }

  [Fact]
  public void SetNumCoinsCollected()
  {
    _repo.SetNumCoinsCollected(5);

    _repo.NumCoinsCollected.Value.ShouldBe(5);
  }

  [Fact]
  public void GlobalCameraDirection()
  {
    _repo.SetCameraBasis(Basis.Identity);

    _repo.GlobalCameraDirection.ShouldBe(Vector3.Forward);
  }
}
