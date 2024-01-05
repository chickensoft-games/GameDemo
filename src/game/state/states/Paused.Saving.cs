namespace GameDemo;

public partial class GameLogic {
  public partial record State {
    public record Saving : Paused, IGet<Input.GameSaveCompleted> {
      public Saving() {
        OnAttach(() => Get<IGameRepo>().GameSaveCompleted += OnGameSaveCompleted);
        OnDetach(() => Get<IGameRepo>().GameSaveCompleted -= OnGameSaveCompleted);

        OnEnter<Saving>(
          previous => {
            Context.Output(new Output.ShowPauseSaveOverlay());
            Get<IGameRepo>().OnStartSaving();
          }
        );

        OnExit<Saving>(
          next => Context.Output(new Output.HidePauseSaveOverlay())
        );
      }

      private void OnGameSaveCompleted() =>
        Context.Input(new Input.GameSaveCompleted());

      public IState On(Input.GameSaveCompleted input)
        => new Paused();

      // Make it impossible to leave the pause menu while saving
      public override IState On(Input.PauseButtonPressed input) => this;
    }
  }
}
