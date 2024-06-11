namespace GameDemo;

using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;

public partial class GameLogic {
  public partial record State {
    [Meta]
    public partial record RestartingGame : State {
      public RestartingGame() {
        this.OnEnter(
          () => Get<IAppRepo>().OnExitGame(PostGameAction.RestartGame)
        );
      }
    }
  }
}
