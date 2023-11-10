namespace GameDemo;
public partial class AppLogic {
  public partial record State {
    public record WonGame : InGame {
      public WonGame(IContext context) : base(context) {
        OnEnter<WonGame>(
          (previous) => Context.Output(new Output.ShowPlayerWon())
        );
      }
    }
  }
}
