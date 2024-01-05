namespace GameDemo;

public partial class GameLogic {
  public partial record State {
    public record RestartingGame : State {
      public RestartingGame() {
        OnEnter<RestartingGame>(
          _ => Get<IAppRepo>().OnExitGame(PostGameAction.RestartGame));
      }
    }
  }
}
