namespace GameDemo;

using System;
using Chickensoft.GoDotCollections;

/// <summary>
/// Pure application game logic repository — shared between view-specific logic
/// blocks.
/// </summary>
public interface IAppRepo : IDisposable {
  /// <summary>Mouse captured status.</summary>
  IAutoProp<bool> IsMouseCaptured { get; }
  /// <summary>Number of coins collected by the players.</summary>
  IAutoProp<int> NumCoinsCollected { get; }

  /// <summary>The total number of coins the world started with.</summary>
  IAutoProp<int> NumCoinsAtStart { get; }

  /// <summary>Event invoked when a coin is collected.</summary>
  event Action? CoinCollected;

  /// <summary>Event invoked when a jumpshroom is used to bounce.</summary>
  event Action? JumpshroomUsed;

  /// <summary>Event invoked whenever the player jumps.</summary>
  event Action? Jumped;

  /// <summary>
  /// Event invoked when the game is about to start.
  /// </summary>
  event Action? GameStarting;

  /// <summary>Event invoked when the game ends.</summary>
  event Action<GameOverReason>? GameEnded;

  /// <summary>Event invoked when the game is paused.</summary>
  event Action? GamePaused;

  /// <summary>Event invoked when the game is resumed.</summary>
  event Action? GameResumed;

  /// <summary>Event invoked when the game should be saved.</summary>
  event Action? GameSaveRequested;
  event Action? GameSaveCompleted;

  /// <summary>Event invoked when the splash screen is skipped.</summary>
  event Action? SplashScreenSkipped;

  /// <summary>Event invoked when the main menu is entered.</summary>
  event Action? MainMenuEntered;

  /// <summary>Inform the app that the game is about to begin.</summary>
  void OnStartGame();

  /// <summary>Inform the app that a jumpshroom was used.</summary>
  void OnJumpshroomUsed();

  /// <summary>Inform the game that the player is collecting a coin.</summary>
  /// <param name="coin">Coin that is being collected.</param>
  void StartCoinCollection(ICoin coin);

  /// <summary>Inform the game that the player collected a coin.</summary>
  /// <param name="coin">Coin that was collected.</param>
  void OnFinishCoinCollection(ICoin coin);

  /// <summary>Tells the app how many coins the game world contains.</summary>
  /// <param name="numCoinsAtStart">Initial number of coins.</param>
  void OnNumCoinsAtStart(int numCoinsAtStart);

  /// <summary>Inform the application that the game ended.</summary>
  /// <param name="reason">Reason why the game ended.</param>
  void OnGameEnded(GameOverReason reason);

  /// <summary>Tells the app that the main menu was entered.</summary>
  void OnMainMenuEntered();

  /// <summary>Pauses the game and releases the mouse.</summary>
  void Pause();

  /// <summary>Resumes the game and recaptures the mouse.</summary>
  void Resume();

  /// <summary>Skips the splash screen.</summary>
  void SkipSplashScreen();

  /// <summary>Tells the app that the player jumped.</summary>
  void Jump();

  /// <summary>Starts the saving process.</summary>
  void StartSaving();
}

/// <summary>
/// Pure application game logic repository — shared between view-specific logic
/// blocks.
/// </summary>
public class AppRepo : IAppRepo {
  public IAutoProp<bool> IsMouseCaptured => _isMouseCaptured;
  private readonly AutoProp<bool> _isMouseCaptured;
  public IAutoProp<int> NumCoinsCollected => _numCoinsCollected;
  private readonly AutoProp<int> _numCoinsCollected;
  public IAutoProp<int> NumCoinsAtStart => _numCoinsAtStart;
  private readonly AutoProp<int> _numCoinsAtStart;

  public event Action? CoinCollected;
  public event Action? JumpshroomUsed;
  public event Action? GameStarting;
  public event Action<GameOverReason>? GameEnded;
  public event Action? GamePaused;
  public event Action? GameResumed;
  public event Action? GameSaveRequested;
  public event Action? GameSaveCompleted;
  public event Action? SplashScreenSkipped;
  public event Action? Jumped;
  public event Action? MainMenuEntered;

  private bool _disposedValue;

  private int _coinsBeingCollected;

  public AppRepo() {
    _isMouseCaptured = new AutoProp<bool>(false);
    _numCoinsCollected = new AutoProp<int>(0);
    _numCoinsAtStart = new AutoProp<int>(0);
  }

  internal AppRepo(
    AutoProp<bool> isMouseCaptured,
    AutoProp<int> numCoinsCollected,
    AutoProp<int> numCoinsAtStart
  ) {
    _isMouseCaptured = isMouseCaptured;
    _numCoinsCollected = numCoinsCollected;
    _numCoinsAtStart = numCoinsAtStart;
  }

  public void OnStartGame() {
    _isMouseCaptured.OnNext(true);
    GameStarting?.Invoke();
    Reset();
  }

  public void OnJumpshroomUsed() {
    JumpshroomUsed?.Invoke();
  }

  public void StartCoinCollection(ICoin coin) {
    _coinsBeingCollected++;
    _numCoinsCollected.OnNext(_numCoinsCollected.Value + 1);
    CoinCollected?.Invoke();
  }

  public void OnFinishCoinCollection(ICoin coin) {
    _coinsBeingCollected--;

    if (
      _coinsBeingCollected == 0 &&
      _numCoinsCollected.Value >= _numCoinsAtStart.Value
    ) {
      OnGameEnded(GameOverReason.PlayerWon);
    }
  }

  public void OnNumCoinsAtStart(int numCoinsAtStart) =>
    _numCoinsAtStart.OnNext(numCoinsAtStart);

  public void OnGameEnded(GameOverReason reason) {
    _isMouseCaptured.OnNext(false);
    GameEnded?.Invoke(reason);
  }

  public void Pause() {
    _isMouseCaptured.OnNext(false);
    GamePaused?.Invoke();
  }

  public void Resume() {
    _isMouseCaptured.OnNext(true);
    GameResumed?.Invoke();
  }

  public void SkipSplashScreen() => SplashScreenSkipped?.Invoke();

  public void OnMainMenuEntered() => MainMenuEntered?.Invoke();

  public void Jump() => Jumped?.Invoke();

  public void StartSaving() {
    GameSaveRequested?.Invoke();
    // TODO: Remove this later
    GameSaveCompleted?.Invoke();
  }

  #region Internals
  private void Reset() {
    _numCoinsCollected.OnNext(0);
    _coinsBeingCollected = 0;
  }

  protected void Dispose(bool disposing) {
    if (!_disposedValue) {
      if (disposing) {
        // Dispose managed objects.
        _isMouseCaptured.Dispose();
        _numCoinsCollected.OnCompleted();
        _numCoinsAtStart.Dispose();
      }

      _disposedValue = true;
    }
  }

  public void Dispose() {
    Dispose(disposing: true);
    GC.SuppressFinalize(this);
  }
  #endregion Internals
}
