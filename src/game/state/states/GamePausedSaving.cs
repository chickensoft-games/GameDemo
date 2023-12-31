namespace GameDemo;

public partial class GameLogic {
  public partial record State {
    public record GamePausedSaving : GamePaused, IGet<Input.GameSaveCompleted> {
      public GamePausedSaving() {
        OnAttach(() => Get<IGameRepo>().GameSaveCompleted += OnGameSaveCompleted);
        OnDetach(() => Get<IGameRepo>().GameSaveCompleted -= OnGameSaveCompleted);

        OnEnter<GamePausedSaving>(
          previous => {
            Context.Output(new Output.ShowPauseSaveOverlay());
            Get<IGameRepo>().StartSaving();
          }
        );

        OnExit<GamePausedSaving>(
          next => Context.Output(new Output.HidePauseSaveOverlay())
        );
      }

      private void OnGameSaveCompleted() =>
        Context.Input(new Input.GameSaveCompleted());

      public IState On(Input.GameSaveCompleted input)
        => new GamePaused();

      // Make it impossible to leave the pause menu while saving
      public override IState On(Input.PauseButtonPressed input) => this;
    }
  }
}
