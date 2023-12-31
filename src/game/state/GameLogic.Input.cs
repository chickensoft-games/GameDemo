namespace GameDemo;

public partial class GameLogic {
  public static class Input {
    public readonly record struct Initialize(int NumCoinsInWorld);

    public readonly record struct GameOver(GameOverReason Reason);

    public readonly record struct PauseButtonPressed;

    public readonly record struct PauseMenuTransitioned;

    public readonly record struct GameSaveRequested;

    public readonly record struct GameSaveCompleted;

    public readonly record struct GoToMainMenu;

    public readonly record struct StartGame;
  }
}
