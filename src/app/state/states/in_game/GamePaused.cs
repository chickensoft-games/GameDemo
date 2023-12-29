namespace GameDemo;

public partial class AppLogic {
  public partial record State {
    public record GamePaused : InGame,
    IGet<Input.PauseButtonPressed>, IGet<Input.GameSaveRequested> {
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

      public virtual IState On(Input.PauseButtonPressed input)
        => new ResumingGame();

      public IState On(Input.GameSaveRequested input) {
        return new GamePausedSaving();
      }
    }
  }
}
