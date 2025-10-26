namespace GameDemo;

public partial class AppLogic
{
  public static class Output
  {
    public readonly record struct FadeToBlack;

    public readonly record struct SetupGameScene();

    public readonly record struct ShowGame(bool ShouldLoadExistingGame);

    public readonly record struct HideGame;

    public readonly record struct PlayGame;

    public readonly record struct ShowMainMenu;

    public readonly record struct RemoveExistingGame;

    public readonly record struct ShowSplashScreen;

    public readonly record struct HideSplashScreen;

    public readonly record struct StartLoadingSaveFile;
  }
}
