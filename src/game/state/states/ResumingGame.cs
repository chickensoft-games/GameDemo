namespace GameDemo;
public partial class AppLogic {
  public partial record State {
    public record ResumingGame : InGame, IGet<Input.PauseMenuTransitioned> {
      public ResumingGame() {
        OnEnter<ResumingGame>((previous) => Get<IAppRepo>().Resume());
        OnExit<ResumingGame>(
          (next) => Context.Output(new Output.DisablePauseMenu())
        );
      }

      public IState On(Input.PauseMenuTransitioned input) =>
        new PlayingGame();
    }
  }
}
