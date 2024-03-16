namespace GameDemo;

using Chickensoft.LogicBlocks;

public partial class GameLogic {
  public partial record State {
    public record Quit : State {
      public Quit() {
        this.OnEnter(
          () => Get<IAppRepo>().OnExitGame(PostGameAction.GoToMainMenu)
        );
      }
    }
  }
}
