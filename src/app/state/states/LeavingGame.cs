namespace GameDemo;

public partial class AppLogic {
  public partial record State {
    public record LeavingGame : State {
      public LeavingGame() {
        OnEnter<LeavingGame>(
          previous => Context.Output(new Output.FadeOut())
        );
        OnExit<LeavingGame>(
          next => Context.Output(new Output.RemoveExistingGame())
        );
      }
    }
  }
}
