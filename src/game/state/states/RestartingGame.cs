namespace GameDemo;

partial class GameLogic {
  partial record State {
    public record RestartingGame : State {
      public RestartingGame() {
        OnEnter<RestartingGame>(
          _ => Get<IAppRepo>().OnExitGame(PostGameAction.RestartGame));
      }
    }
  }
}
