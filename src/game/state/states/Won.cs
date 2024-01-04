namespace GameDemo;

public partial class GameLogic {
  public partial record State {
    public record Won : State {
      public Won() {
        OnEnter<Won>(
          previous => Context.Output(new Output.ShowPlayerWon())
        );
      }
    }
  }
}
