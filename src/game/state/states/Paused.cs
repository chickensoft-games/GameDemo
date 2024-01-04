namespace GameDemo;

public partial class GameLogic {
  public partial record State {
    public record Paused : State,
      IGet<Input.PauseButtonPressed>, IGet<Input.GoToMainMenu>, IGet<Input.GameSaveRequested> {
      public Paused() {
        OnEnter<Paused>(
          _ => {
            Context.Output(new Output.ShowPauseMenu());
            Context.Output(new Output.SetPauseMode(true));
            Get<IGameRepo>().Pause();
          }
        );
        OnExit<Paused>(
          _ => {
            Context.Output(new Output.ExitPauseMenu());
            Context.Output(new Output.SetPauseMode(false));
          }
        );
      }

      public virtual IState On(Input.PauseButtonPressed input)
        => new Resuming();

      public IState On(Input.GameSaveRequested input) => new Saving();

      public IState On(Input.GoToMainMenu input) => new Quit();
    }
  }
}
