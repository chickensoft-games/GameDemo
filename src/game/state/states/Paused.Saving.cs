namespace GameDemo;

using Chickensoft.LogicBlocks;

public partial class GameLogic {
  public partial record State {
    public record Saving : Paused, IGet<Input.SaveCompleted> {
      public Saving() {
        OnAttach(() => Get<IGameRepo>().SaveCompleted += OnSaveCompleted);
        OnDetach(() => Get<IGameRepo>().SaveCompleted -= OnSaveCompleted);

        this.OnEnter(
          () => {
            Output(new Output.ShowPauseSaveOverlay());
            Get<IGameRepo>().Save();
          }
        );

        this.OnExit(() => Output(new Output.HidePauseSaveOverlay()));
      }

      public void OnSaveCompleted() =>
        Input(new Input.SaveCompleted());

      public IState On(in Input.SaveCompleted input)
        => new Paused();

      // Make it impossible to leave the pause menu while saving
      public override IState On(in Input.PauseButtonPressed input) => this;
    }
  }
}
