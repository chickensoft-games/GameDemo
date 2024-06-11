namespace GameDemo;

using Chickensoft.Introspection;

public partial class AppLogic {
  public partial record State {
    [Meta]
    public partial record LeavingGame : State, IGet<Input.FadeOutFinished> {
      public PostGameAction PostGameAction { get; set; } = PostGameAction.RestartGame;

      public Transition On(in Input.FadeOutFinished input) {
        // We are either supposed to restart the game or go back to the main
        // menu. More complex games might have more post-game destinations,
        // but it's pretty simple for us.
        Output(new Output.RemoveExistingGame());

        if (PostGameAction is not PostGameAction.RestartGame) {
          return To<MainMenu>();
        }

        Output(new Output.SetupGameScene());
        return To<InGame>();
      }
    }
  }
}
