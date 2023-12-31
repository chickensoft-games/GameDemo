namespace GameDemo;

public partial class GameLogic {
  public partial record State {
    public record PlayingGame : InGame, IGet<Input.PauseButtonPressed> {
      public IState On(Input.PauseButtonPressed input)
        => new GamePaused();
    }
  }
}
