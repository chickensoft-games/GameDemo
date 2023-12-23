namespace GameDemo;
public partial class AppLogic {
  public partial record State {
    public record GamePaused : InGame, IGet<Input.PauseButtonPressed> {
      public GamePaused() {
        OnEnter<GamePaused>(
          (previous) => {
            Context.Output(new Output.ShowPauseMenu());
            Get<IAppRepo>().Pause();
          }
        );
        OnExit<GamePaused>(
          (next) => Context.Output(new Output.HidePauseMenu())
        );
      }

      public IState On(Input.PauseButtonPressed input)
        => new ResumingGame();
    }
  }
}
