namespace GameDemo.Tests;

using Chickensoft.GoDotCollections;
using Chickensoft.GoDotTest;
using Godot;
using Moq;
using Shouldly;

public class GameRepoTest : TestClass {
  private AutoProp<bool> _isMouseCaptured = default!;
  private AutoProp<bool> _isPaused = default!;
  private AutoProp<Vector3> _playerGlobalPosition = default!;
  private AutoProp<Basis> _cameraBasis = default!;
  private AutoProp<int> _numCoinsCollected = default!;
  private AutoProp<int> _numCoinsAtStart = default!;

  private GameRepo _repo = default!;

  public GameRepoTest(Node testScene) : base(testScene) { }

  [Setup]
  public void Setup() {
    _isMouseCaptured = new(false);
    _isPaused = new(false);
    _playerGlobalPosition = new(Vector3.Zero);
    _cameraBasis = new(Basis.Identity);
    _numCoinsCollected = new(0);
    _numCoinsAtStart = new(0);

    _repo = new(
      _isMouseCaptured,
      _isPaused,
      _playerGlobalPosition,
      _cameraBasis,
      _numCoinsCollected,
      _numCoinsAtStart
    );
  }

  [Cleanup]
  public void Cleanup() => _repo.Dispose();

  [Test]
  public void Initializes() {
    var repo = new GameRepo();
    repo.ShouldBeAssignableTo<IGameRepo>();
  }

  [Test]
  public void SetPlayerGlobalPosition() {
    _repo.SetPlayerGlobalPosition(Vector3.One);

    _repo.PlayerGlobalPosition.Value.ShouldBe(Vector3.One);
  }

  [Test]
  public void SetIsMouseCaptured() {
    _repo.SetIsMouseCaptured(true);

    _repo.IsMouseCaptured.Value.ShouldBe(true);
  }

  [Test]
  public void SetCameraBasis() {
    _repo.SetCameraBasis(Basis.Identity);

    _repo.CameraBasis.Value.ShouldBe(Basis.Identity);
  }

  [Test]
  public void StartCoinCollection() {
    var coin = new Mock<ICoin>();
    var called = false;

    _repo.CoinCollected += () => called = true;

    _repo.StartCoinCollection(coin.Object);

    _numCoinsCollected.Value.ShouldBe(1);

    called.ShouldBeTrue();
  }

  [Test]
  public void StartCoinCollectionDoesNotInvokeEventIfNoListeners() {
    var coin = new Mock<ICoin>();

    _repo.StartCoinCollection(coin.Object);

    _numCoinsCollected.Value.ShouldBe(1);
  }

  [Test]
  public void OnFinishCoinCollectionTriggersWin() {
    var coin = new Mock<ICoin>();
    var coins = 0;
    GameOverReason gameOverReason = default!;

    void gameEnded(GameOverReason reason) => gameOverReason = reason;

    void coinCollected() => coins++;

    _repo.Ended += gameEnded;
    _repo.CoinCollected += coinCollected;
    _repo.StartCoinCollection(coin.Object);

    _repo.NumCoinsCollected.Value.ShouldBe(1);
    coins.ShouldBe(1);

    _repo.OnFinishCoinCollection(coin.Object);

    gameOverReason.ShouldBe(GameOverReason.Won);

    _repo.Ended -= gameEnded;
    _repo.CoinCollected -= coinCollected;
  }

  [Test]
  public void OnFinishCoinCollectionDoesNothingIfNonZeroAmountOfCoins() {
    var coin = new Mock<ICoin>();
    Should.NotThrow(() => _repo.OnFinishCoinCollection(coin.Object));
  }

  [Test]
  public void OnJumpInvokesEvent() {
    Should.NotThrow(_repo.OnJump);

    var called = false;
    _repo.Jumped += () => called = true;

    _repo.OnJump();

    called.ShouldBeTrue();
  }

  [Test]
  public void SaveInvokesEvents() {
    Should.NotThrow(_repo.Save);

    var called = 0;
    _repo.SaveRequested += () => called++;
    _repo.SaveCompleted += () => called++;

    _repo.Save();

    called.ShouldBe(2);
  }

  [Test]
  public void OnGameEndedPausesAndInvokesGameEnded() {
    var called = false;
    _repo.Ended += _ => called = true;

    _repo.OnGameEnded(GameOverReason.Won);

    called.ShouldBeTrue();
    _repo.IsPaused.Value.ShouldBeTrue();
    _repo.IsMouseCaptured.Value.ShouldBe(false);
  }

  [Test]
  public void OnGameEndedDoesNotInvokeEventIfNoListeners() =>
    Should.NotThrow(() => _repo.OnGameEnded(GameOverReason.Won));

  [Test]
  public void Pause() {
    _repo.Pause();
    _isMouseCaptured.Value.ShouldBe(false);
    _isPaused.Value.ShouldBe(true);
  }

  [Test]
  public void Resume() {
    _repo.Resume();
    _isMouseCaptured.Value.ShouldBe(true);
    _isPaused.Value.ShouldBe(false);
  }

  [Test]
  public void OnJumpshroomUsedInvokesEvent() {
    var called = 0;

    void onJumpshroomUsed() => called++;

    _repo.OnJumpshroomUsed();
    _repo.JumpshroomUsed += onJumpshroomUsed;
    _repo.OnJumpshroomUsed();

    called.ShouldBe(1);
  }

  [Test]
  public void SetNumCoinsAtStart() {
    _repo.SetNumCoinsAtStart(5);

    _repo.NumCoinsAtStart.Value.ShouldBe(5);
  }

  [Test]
  public void GlobalCameraDirection() {
    _repo.SetCameraBasis(Basis.Identity);

    _repo.GlobalCameraDirection.ShouldBe(Vector3.Forward);
  }
}
