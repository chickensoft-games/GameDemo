namespace GameDemo;

using Chickensoft.LogicBlocks;

public partial class GameLogic {
  public partial record State {
    public record Playing : State,
    IGet<Input.EndGame>, IGet<Input.PauseButtonPressed> {
      public Playing() {
        this.OnEnter(
          () => {
            Output(new Output.StartGame());
            Get<IGameRepo>().SetIsMouseCaptured(true);
          }
        );

        OnAttach(() => Get<IGameRepo>().Ended += OnEnded);
        OnDetach(() => Get<IGameRepo>().Ended -= OnEnded);
      }

      public void OnEnded(GameOverReason reason)
        => Input(new Input.EndGame(reason));

      public IState On(in Input.EndGame input) {
        Get<IGameRepo>().Pause();

        return input.Reason switch {
          GameOverReason.Won => new Won(),
          GameOverReason.Lost => new Lost(),
          _ => new Quit()
        };
      }

      public IState On(in Input.PauseButtonPressed input) => new Paused();
    }
  }
}
