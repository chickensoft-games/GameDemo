namespace GameDemo;

using System;

public partial class GameLogic {
  public partial record State {
    public record PlayingGame :
      State, IGet<Input.GoToMainMenu>, IGet<Input.GameOver>, IGet<Input.PauseButtonPressed> {
      public PlayingGame() {
        OnEnter<PlayingGame>(
          _ => {
            Context.Output(new Output.StartGame());
            Get<IGameRepo>().SetIsMouseCaptured(true);
          }
        );

        OnAttach(() => Get<IGameRepo>().GameEnded += OnGameOver);
        OnDetach(() => Get<IGameRepo>().GameEnded -= OnGameOver);
      }

      public void OnGameOver(GameOverReason reason)
        => Context.Input(new Input.GameOver(reason));

      public IState On(Input.GameOver input) {
        Get<IGameRepo>().Pause();

        return input.Reason switch {
          GameOverReason.PlayerWon => new WonGame(),
          GameOverReason.PlayerDied => new LostGame(),
          GameOverReason.Exited => new QuitGame(),
          _ => throw new InvalidOperationException()
        };
      }

      public IState On(Input.GoToMainMenu input) => new QuitGame();
      public IState On(Input.PauseButtonPressed input) => new GamePaused();
    }
  }
}
