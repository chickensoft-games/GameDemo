namespace GameDemo;

public partial class GameLogic {
  public partial record State {
    public record Quit : State {
      public Quit() {
        OnEnter<Quit>(_ => Get<IAppRepo>().EndGame(GameOverReason.Exited));
      }
    }
  }
}
