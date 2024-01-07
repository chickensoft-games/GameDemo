namespace GameDemo;

partial class GameLogic {
  partial record State {
    public record Playing : State, IGet<Input.EndGame>, IGet<Input.PauseButtonPressed> {
      public Playing() {
        OnEnter<Playing>(
          _ => {
            Context.Output(new Output.StartGame());
            Get<IGameRepo>().SetIsMouseCaptured(true);
          }
        );

        OnAttach(() => Get<IGameRepo>().Ended += OnEnded);
        OnDetach(() => Get<IGameRepo>().Ended -= OnEnded);
      }

      public void OnEnded(GameOverReason reason)
        => Context.Input(new Input.EndGame(reason));

      public IState On(Input.EndGame input) {
        Get<IGameRepo>().Pause();

        return input.Reason switch {
          GameOverReason.Won => new Won(),
          GameOverReason.Lost => new Lost(),
          _ => new Quit()
        };
      }

      public IState On(Input.PauseButtonPressed input) => new Paused();
    }
  }
}
