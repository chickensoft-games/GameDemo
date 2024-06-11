namespace GameDemo;

using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;

public partial class GameLogic {
  public partial record State {
    [Meta]
    public partial record Saving : Paused, IGet<Input.SaveCompleted> {
      public Saving() {
        this.OnEnter(
          () => {
            Output(new Output.ShowPauseSaveOverlay());
            Output(new Output.StartSaving());
          }
        );

        this.OnExit(() => Output(new Output.HidePauseSaveOverlay()));
      }

      public Transition On(in Input.SaveCompleted input) => To<Paused>();

      // Make it impossible to leave the pause menu while saving
      public override Transition On(in Input.PauseButtonPressed input) =>
        ToSelf();
    }
  }
}
