namespace GameDemo;

public partial class AppLogic {
  public partial record State {
    public record LeavingGame : State, IGet<Input.FadeOutFinished> {
      public bool ShouldRestartGame { get; }

      public LeavingGame(bool shouldRestartGame) {
        ShouldRestartGame = shouldRestartGame;

        OnExit<LeavingGame>(
          _ => Context.Output(new Output.RemoveExistingGame())
        );
      }

      public IState On(Input.FadeOutFinished input) {
        if (ShouldRestartGame) {
          return new RestartingGame();
        }

        return new MainMenu();
      }
    }
  }
}
