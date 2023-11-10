namespace GameDemo.Tests;

using Chickensoft.GoDotCollections;
using Chickensoft.GoDotTest;
using Godot;
using Moq;
using Shouldly;

public class AppRepoTest : TestClass {
  private AutoProp<bool> _isMouseCaptured = default!;
  private AutoProp<int> _numCoinsCollected = default!;
  private AutoProp<int> _numCoinsAtStart = default!;

  private AppRepo _repo = default!;

  public AppRepoTest(Node testScene) : base(testScene) { }

  [Setup]
  public void Setup() {
    _isMouseCaptured = new(false);
    _numCoinsCollected = new(0);
    _numCoinsAtStart = new(0);

    _repo = new(
      _isMouseCaptured,
      _numCoinsCollected,
      _numCoinsAtStart
    );
  }

  [Cleanup]
  public void Cleanup() => _repo.Dispose();

  [Test]
  public void Initializes() {
    var repo = new AppRepo();
    repo.ShouldBeAssignableTo<IAppRepo>();
  }

  [Test]
  public void GameStartingPreparesGame() {
    var called = false;
    void gameStarting() => called = true;

    // invoke event without handlers to cover null check
    _repo.OnStartGame();

    _repo.GameStarting += gameStarting;

    _repo.OnStartGame();

    _isMouseCaptured.Value.ShouldBe(true);
    _numCoinsCollected.Value.ShouldBe(0);
    _numCoinsAtStart.Value.ShouldBe(0);

    called.ShouldBe(true);

    _repo.GameStarting -= gameStarting;
  }

  [Test]
  public void DoesNotInvokesCoinCollectionIfNull() {
    var coin = new Mock<ICoin>();
    Should.NotThrow(() => _repo.StartCoinCollection(coin.Object));
  }

  [Test]
  public void CollectsCoinsAndTriggersWinCondition() {
    var coin = new Mock<ICoin>();
    var coins = 0;
    GameOverReason gameOverReason = default!;
    void gameEnded(GameOverReason reason) => gameOverReason = reason;
    void coinCollected() => coins++;

    _repo.GameEnded += gameEnded;
    _repo.CoinCollected += coinCollected;
    _repo.StartCoinCollection(coin.Object);

    _repo.NumCoinsCollected.Value.ShouldBe(1);
    coins.ShouldBe(1);

    _repo.OnFinishCoinCollection(coin.Object);

    gameOverReason.ShouldBe(GameOverReason.PlayerWon);

    _repo.GameEnded -= gameEnded;
    _repo.CoinCollected -= coinCollected;
  }

  [Test]
  public void CoinCollectedDoesNothingIfWinConditionNotMet() {
    var coin = new Mock<ICoin>();
    var called = false;

    _numCoinsAtStart.OnNext(2);

    void gameEnded(GameOverReason reason) => called = true;
    _repo.GameEnded += gameEnded;

    _repo.OnFinishCoinCollection(coin.Object);

    called.ShouldBe(false);

    _repo.GameEnded -= gameEnded;
  }

  [Test]
  public void SetNumCoinsAtStart() {
    _repo.OnNumCoinsAtStart(5);

    _repo.NumCoinsAtStart.Value.ShouldBe(5);
  }

  [Test]
  public void GameEnded() {
    _repo.OnGameEnded(GameOverReason.PlayerWon);
    _repo.IsMouseCaptured.Value.ShouldBe(false);
  }

  [Test]
  public void Pause() {
    _repo.Pause();
    _repo.IsMouseCaptured.Value.ShouldBe(false);

    var called = false;
    void gamePaused() => called = true;
    _repo.GamePaused += gamePaused;

    _repo.Pause();

    called.ShouldBe(true);
  }

  [Test]
  public void Resume() {
    _repo.Resume();
    _repo.IsMouseCaptured.Value.ShouldBe(true);

    var called = false;
    void gameResumed() => called = true;
    _repo.GameResumed += gameResumed;

    _repo.Resume();

    called.ShouldBe(true);
  }

  [Test]
  public void SkipSplashScreen() {
    var called = false;
    void splashScreenSkipped() => called = true;

    // invoke event without handlers to cover null check
    _repo.SkipSplashScreen();

    _repo.SplashScreenSkipped += splashScreenSkipped;

    _repo.SkipSplashScreen();

    called.ShouldBe(true);
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
  public void OnMainMenuEnteredInvokesEvent() {
    var called = 0;
    void onMainMenuEntered() => called++;

    _repo.OnMainMenuEntered();
    _repo.MainMenuEntered += onMainMenuEntered;
    _repo.OnMainMenuEntered();

    called.ShouldBe(1);
  }

  [Test]
  public void JumpInvokesEvent() {
    var called = 0;
    void onJump() => called++;

    _repo.Jump();
    _repo.Jumped += onJump;
    _repo.Jump();

    called.ShouldBe(1);
  }
}
