namespace GameDemo;

public partial class GameLogic {
  public partial record State {
    public record Paused : State,
      IGet<Input.PauseButtonPressed>, IGet<Input.GoToMainMenu>, IGet<Input.SaveRequested> {
      public Paused() {
        OnEnter<Paused>(
          _ => {
            Get<IGameRepo>().Pause();
            Context.Output(new Output.ShowPauseMenu());
          }
        );
        // We don't resume on exit because we can leave this state for
        // a menu and we want to remain paused.
        OnExit<Paused>(
          _ => Context.Output(new Output.ExitPauseMenu())
        );
      }

      public virtual IState On(Input.PauseButtonPressed input)
        => new Resuming();

      public IState On(Input.SaveRequested input) => new Saving();

      public IState On(Input.GoToMainMenu input) => new Quit();
    }
  }
}
