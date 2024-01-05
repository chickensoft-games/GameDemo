namespace GameDemo;

public partial class AppLogic {
  public partial record State {
    public record LeavingGame : State, IGet<Input.FadeOutFinished> {
      public PostGameAction PostGameAction { get; }

      public LeavingGame(PostGameAction postGameAction) {
        PostGameAction = postGameAction;
      }

      public IState On(Input.FadeOutFinished input) {
        // We are either supposed to restart the game or go back to the main
        // menu. More complex games might have more post-game destinations,
        // but it's pretty simple for us.
        Context.Output(new Output.RemoveExistingGame());

        if (PostGameAction is not PostGameAction.RestartGame) {
          return new MainMenu();
        }

        Context.Output(new Output.LoadGame());
        return new InGame();
      }
    }
  }
}
