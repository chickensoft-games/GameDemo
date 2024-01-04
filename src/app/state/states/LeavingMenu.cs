namespace GameDemo;

public partial class AppLogic {
  public partial record State {
    public record LeavingMenu : State, IGet<Input.FadeOutFinished> {
      public LeavingMenu() {
        OnEnter<LeavingMenu>(
          previous => Context.Output(new Output.FadeToBlack())
        );

        OnExit<LeavingMenu>(_ => Get<IAppRepo>().OnStartGame());
      }

      public IState On(Input.FadeOutFinished input) =>
        new InGame();
    }
  }
}
