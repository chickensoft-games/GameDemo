namespace GameDemo;

public abstract partial record AppLogicState
{
  public static class Output
  {
    public readonly record struct FadeToBlack;

    public readonly record struct SetupGameScene;

    public readonly record struct ShowGame;

    public readonly record struct HideGame;

    public readonly record struct PlayGame;

    public readonly record struct ShowMainMenu;

    public readonly record struct RemoveExistingGame;

    public readonly record struct ShowSplashScreen;

    public readonly record struct HideSplashScreen;

    public readonly record struct StartLoadingSaveFile;
  }
}
