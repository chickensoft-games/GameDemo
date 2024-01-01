namespace GameDemo;

public partial class GameLogic {
  public partial record State {
    public record PlayingGame : InGame;
  }
}
