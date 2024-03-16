namespace GameDemo;

using Chickensoft.LogicBlocks;

public partial class GameLogic {
  public partial record State {
    public record Paused : State,
      IGet<Input.PauseButtonPressed>,
      IGet<Input.GoToMainMenu>,
      IGet<Input.SaveRequested> {
      public Paused() {
        this.OnEnter(
          () => {
            Get<IGameRepo>().Pause();
            Output(new Output.ShowPauseMenu());
          }
        );

        // We don't resume on exit because we can leave this state for
        // a menu and we want to remain paused.
        this.OnExit(() => Output(new Output.ExitPauseMenu()));
      }

      public virtual IState On(in Input.PauseButtonPressed input)
        => new Resuming();

      public IState On(in Input.SaveRequested input) => new Saving();

      public IState On(in Input.GoToMainMenu input) => new Quit();
    }
  }
}
