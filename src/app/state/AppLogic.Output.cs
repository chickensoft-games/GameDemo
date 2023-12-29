namespace GameDemo;

public partial class AppLogic {
  public static class Output {
    public readonly record struct FadeOut;
    public readonly record struct LoadGame;
    public readonly record struct ShowGame;
    public readonly record struct PlayGame;
    public readonly record struct ShowPlayerDied;
    public readonly record struct ShowPlayerWon;
    public readonly record struct ShowMainMenu;
    public readonly record struct ShowPauseMenu;
    public readonly record struct HidePauseMenu;
    public readonly record struct ShowPauseSaveOverlay;
    public readonly record struct HidePauseSaveOverlay;
    public readonly record struct DisablePauseMenu;
    public readonly record struct RemoveExistingGame;
    public readonly record struct CaptureMouse(bool IsMouseCaptured);
    public readonly record struct ShowSplashScreen;
    public readonly record struct HideSplashScreen;
  }
}
