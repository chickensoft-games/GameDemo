namespace GameDemo;

public partial class AppLogic {
  public static class Input {
    public readonly record struct FadeInFinished;
    public readonly record struct FadeOutFinished;
    public readonly record struct StartGame;
    public readonly record struct GameOver(GameOverReason Reason);
    public readonly record struct GoToMainMenu;
    public readonly record struct PauseButtonPressed;
    public readonly record struct PauseMenuTransitioned;
  }
}
