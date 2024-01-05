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
  event Action? GameEntered;

  /// <summary>
  ///   Event invoked when the game is about to end.
  /// </summary>
  event Action<PostGameAction>? GameExited;

  /// <summary>Event invoked when the splash screen is skipped.</summary>
  event Action? SplashScreenSkipped;

  /// <summary>Event invoked when the main menu is entered.</summary>
  event Action? MainMenuEntered;

  /// <summary>Inform the app that the game should be shown.</summary>
  void OnEnterGame();

  /// <summary>Inform the app that the game should be exited.</summary>
  /// <param name="action">Action to take following the end of the game.</param>
  void OnExitGame(PostGameAction action);

  /// <summary>Tells the app that the main menu was entered.</summary>
  void OnMainMenuEntered();

  /// <summary>Skips the splash screen.</summary>
  void SkipSplashScreen();
}

/// <summary>
///   Pure application game logic repository — shared between view-specific logic
///   blocks.
/// </summary>
public class AppRepo : IAppRepo {
  public event Action? SplashScreenSkipped;
  public event Action? MainMenuEntered;
  public event Action? GameEntered;
  public event Action<PostGameAction>? GameExited;

  private bool _disposedValue;

  public void SkipSplashScreen() => SplashScreenSkipped?.Invoke();
  public void EndGame(PostGameAction reason) => GameExited?.Invoke(reason);

  public void OnMainMenuEntered() => MainMenuEntered?.Invoke();

  public void OnEnterGame() => GameEntered?.Invoke();
  public void OnExitGame(PostGameAction action) => GameExited?.Invoke(action);

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
