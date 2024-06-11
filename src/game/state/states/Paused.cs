namespace GameDemo;

using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;

public partial class GameLogic {
  public partial record State {
    [Meta]
    public partial record Paused : State,
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

      public virtual Transition On(in Input.PauseButtonPressed input)
        => To<Resuming>();

      public Transition On(in Input.SaveRequested input) => To<Saving>();

      public Transition On(in Input.GoToMainMenu input) => To<Quit>();
    }
  }
}
