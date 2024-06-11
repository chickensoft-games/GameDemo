namespace GameDemo;

using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;

public partial class GameLogic {
  public partial record State {
    [Meta]
    public partial record Playing : State,
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

      public Transition On(in Input.EndGame input) {
        Get<IGameRepo>().Pause();

        return input.Reason switch {
          GameOverReason.Won => To<Won>(),
          GameOverReason.Lost => To<Lost>(),
          _ => To<Quit>()
        };
      }

      public Transition On(in Input.PauseButtonPressed input) => To<Paused>();
    }
  }
}
