namespace GameDemo;

public partial class GameLogic {
  public partial record State {
    public record WonGame : InGame {
      public WonGame() {
        OnEnter<WonGame>(
          previous => Context.Output(new Output.ShowPlayerWon())
        );
      }
    }
  }
}
