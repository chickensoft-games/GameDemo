namespace GameDemo;

public partial class GameLogic {
  public partial record State {
    public record GamePaused : State,
      IGet<Input.PauseButtonPressed>, IGet<Input.GameSaveRequested> {
      public GamePaused() {
        OnEnter<GamePaused>(
          _ => {
            Context.Output(new Output.ShowPauseMenu());
            Context.Output(new Output.SetPauseMode(true));
            Get<IGameRepo>().Pause();
          }
        );
        OnExit<GamePaused>(
          _ => {
            Context.Output(new Output.HidePauseMenu());
            Context.Output(new Output.SetPauseMode(false));
            Get<IGameRepo>().Resume();
          }
        );
      }

      public virtual IState On(Input.PauseButtonPressed input)
        => new ResumingGame();

      public IState On(Input.GameSaveRequested input) => new GamePausedSaving();
    }
  }
}
