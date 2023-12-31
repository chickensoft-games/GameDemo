namespace GameDemo;

using System;

public partial class GameLogic {
  public partial record State {
    public record InGame : State,
      IGet<Input.GoToMainMenu>, IGet<Input.GameOver> {
      public InGame() {
        OnEnter<InGame>(previous => {
          Get<IAppRepo>().OnStartGame();
        });

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
    }
  }
}
