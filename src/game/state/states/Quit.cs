namespace GameDemo;

partial class GameLogic {
  partial record State {
    public record Quit : State {
      public Quit() {
        OnEnter<Quit>(
          _ => Get<IAppRepo>().OnExitGame(PostGameAction.GoToMainMenu)
        );
      }
    }
  }
}
