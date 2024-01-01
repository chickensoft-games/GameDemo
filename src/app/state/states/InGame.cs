namespace GameDemo;

public partial class AppLogic {
  public partial record State {
    public record InGame : State {
      public InGame() {
        OnEnter<InGame>(_ => Context.Output(new Output.ShowGame()));
        OnExit<InGame>(_ => Context.Output(new Output.HideGame()));
      }
    }
  }
}
