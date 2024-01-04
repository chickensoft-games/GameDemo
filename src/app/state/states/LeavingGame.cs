namespace GameDemo;

public partial class AppLogic {
  public partial record State {
    public record LeavingGame : State, IGet<Input.FadeOutFinished> {
      public LeavingGame() {
        OnEnter<LeavingGame>(
          previous => Context.Output(new Output.HideGame())
        );
        OnExit<LeavingGame>(
          next => Context.Output(new Output.RemoveExistingGame())
        );
      }

      public IState On(Input.FadeOutFinished input) => new RestartingGame();
    }
  }
}
