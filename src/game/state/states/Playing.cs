namespace GameDemo;

public partial class GameLogic {
  public partial record State {
    public record Playing : State, IGet<Input.GameOver>, IGet<Input.PauseButtonPressed> {
      public Playing() {
        OnEnter<Playing>(
          _ => {
            Context.Output(new Output.StartGame());
            Get<IGameRepo>().SetIsMouseCaptured(true);
          }
        );

        OnAttach(() => Get<IGameRepo>().Ended += OnGameOver);
        OnDetach(() => Get<IGameRepo>().Ended -= OnGameOver);
      }

      public void OnGameOver(GameOverReason reason)
        => Context.Input(new Input.GameOver(reason));

      public IState On(Input.GameOver input) {
        Get<IGameRepo>().Pause();

        return input.Reason switch {
          GameOverReason.Won => new Won(),
          GameOverReason.Lost => new Lost(),
          GameOverReason.Quit or _ => new Quit()
        };
      }

      public IState On(Input.PauseButtonPressed input) => new Paused();
    }
  }
}
