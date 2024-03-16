namespace GameDemo;

using Chickensoft.LogicBlocks;

public partial class GameLogic {
  public partial record State {
    public record RestartingGame : State {
      public RestartingGame() {
        this.OnEnter(
          () => Get<IAppRepo>().OnExitGame(PostGameAction.RestartGame));
      }
    }
  }
}
