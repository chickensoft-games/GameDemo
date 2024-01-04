namespace GameDemo;

using System;

/// <summary>
///   Pure application game logic repository shared between view-specific logic
///   blocks.
/// </summary>
public interface IAppRepo : IDisposable {
  /// <summary>
  ///   Event invoked when the game is about to start.
  /// </summary>
  event Action? GameStarting;

  /// <summary>
  ///   Event invoked when the game is about to end.
  /// </summary>
  event Action<GameOverReason>? GameEnding;

  /// <summary>Event invoked when the splash screen is skipped.</summary>
  event Action? SplashScreenSkipped;

  /// <summary>Event invoked when the main menu is entered.</summary>
  event Action? MainMenuEntered;

  /// <summary>Inform the app that the game is about to begin.</summary>
  void OnStartGame();

  /// <summary>Tells the app that the main menu was entered.</summary>
  void OnMainMenuEntered();

  /// <summary>Skips the splash screen.</summary>
  void SkipSplashScreen();

  /// <summary>
  ///   End the game with the given reason.
  /// </summary>
  /// <param name="reason">Game over reason.</param>
  void EndGame(GameOverReason reason);
}

/// <summary>
///   Pure application game logic repository — shared between view-specific logic
///   blocks.
/// </summary>
public class AppRepo : IAppRepo {
  public event Action? SplashScreenSkipped;
  public event Action? MainMenuEntered;
  public event Action? GameStarting;
  public event Action<GameOverReason>? GameEnding;

  private bool _disposedValue;

  public void SkipSplashScreen() => SplashScreenSkipped?.Invoke();
  public void EndGame(GameOverReason reason) => GameEnding?.Invoke(reason);

  public void OnMainMenuEntered() => MainMenuEntered?.Invoke();

  public void OnStartGame() => GameStarting?.Invoke();

  #region Internals

  protected void Dispose(bool disposing) {
    if (!_disposedValue) {
      if (disposing) {
        // Dispose managed objects.
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
