namespace GameDemo;

using System;
using Chickensoft.Sync.Primitives;

/// <summary>
///   Pure application game logic repository shared between view-specific logic
///   blocks.
/// </summary>
public interface IAppRepo : IDisposable
{
  IAutoChannel AutoChannel { get; }

  /// <summary>
  ///   Event invoked when the game is about to start.
  /// </summary>
  readonly record struct GameEntering;

  /// <summary>
  ///   Event invoked when the game is about to end.
  /// </summary>
  readonly record struct GameExited(PostGameAction Action);

  /// <summary>Event invoked when the splash screen is skipped.</summary>
  readonly record struct SplashScreenSkipped;

  /// <summary>Event invoked when the main menu is entered.</summary>
  readonly record struct MainMenuEntering;

  /// <summary>Inform the app that the game should be shown.</summary>
  void OnEnteringGame();

  /// <summary>Inform the app that the game should be exited.</summary>
  /// <param name="action">Action to take following the end of the game.</param>
  void OnExitGame(PostGameAction action);

  /// <summary>Tells the app that the main menu was entered.</summary>
  void OnMainMenuEntering();

  /// <summary>Skips the splash screen.</summary>
  void SkipSplashScreen();
}

/// <summary>
///   Pure application game logic repository — shared between view-specific logic
///   blocks.
/// </summary>
public class AppRepo : IAppRepo
{
  private readonly AutoChannel _autoChannel = new();
  public IAutoChannel AutoChannel => _autoChannel;

  private bool _disposedValue;

  public void SkipSplashScreen() => _autoChannel.Send(new IAppRepo.SplashScreenSkipped());

  public void OnMainMenuEntering() => _autoChannel.Send(new IAppRepo.MainMenuEntering());

  public void OnEnteringGame() => _autoChannel.Send(new IAppRepo.GameEntering());
  public void OnExitGame(PostGameAction action) => _autoChannel.Send(new IAppRepo.GameExited(action));

  #region Internals

  protected void Dispose(bool disposing)
  {
    if (!_disposedValue)
    {
      if (disposing)
      {
        _autoChannel.Dispose();
      }

      _disposedValue = true;
    }
  }

  public void Dispose()
  {
    Dispose(disposing: true);
    GC.SuppressFinalize(this);
  }

  #endregion Internals
}
