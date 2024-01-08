namespace GameDemo;

public partial class GameLogic {
  public partial record State {
    public record Saving : Paused, IGet<Input.SaveCompleted> {
      public Saving() {
        OnAttach(() => Get<IGameRepo>().SaveCompleted += OnSaveCompleted);
        OnDetach(() => Get<IGameRepo>().SaveCompleted -= OnSaveCompleted);

        OnEnter<Saving>(
          previous => {
            Context.Output(new Output.ShowPauseSaveOverlay());
            Get<IGameRepo>().Save();
          }
        );

        OnExit<Saving>(
          next => Context.Output(new Output.HidePauseSaveOverlay())
        );
      }

      public void OnSaveCompleted() =>
        Context.Input(new Input.SaveCompleted());

      public IState On(Input.SaveCompleted input)
        => new Paused();

      // Make it impossible to leave the pause menu while saving
      public override IState On(Input.PauseButtonPressed input) => this;
    }
  }
}
