namespace GameDemo;
public partial class AppLogic {
  public partial record State {
    public record GamePausedSaving : GamePaused, IGet<Input.GameSaveCompleted> {
      public GamePausedSaving() {
        OnAttach(() => Get<IAppRepo>().GameSaveCompleted += OnGameSaveCompleted);
        OnDetach(() => Get<IAppRepo>().GameSaveCompleted -= OnGameSaveCompleted);

        OnEnter<GamePausedSaving>(
          (previous) => {
            Context.Output(new Output.ShowPauseSaveOverlay());
            Get<IAppRepo>().StartSaving();
          }
        );

        OnExit<GamePausedSaving>(
          (next) => Context.Output(new Output.HidePauseSaveOverlay())
        );
      }

      private void OnGameSaveCompleted() {
        Context.Input(new Input.GameSaveCompleted());
      }

      public IState On(Input.GameSaveCompleted input)
        => new GamePaused();

      // Make it impossible to leave the pause menu while saving
      public override IState On(Input.PauseButtonPressed input) => this;
    }
  }
}
