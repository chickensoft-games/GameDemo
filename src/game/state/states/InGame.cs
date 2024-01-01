namespace GameDemo;

using System;

public partial class GameLogic {
  public partial record State {
    public record InGame : State,
      IGet<Input.GoToMainMenu>, IGet<Input.GameOver>, IGet<Input.Initialize> {
      public InGame() {
        OnEnter<InGame>(
          _ => {
            Get<IAppRepo>().OnStartGame();
            Context.Output(new Output.ChangeToThirdPersonCamera());
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

      public IState On(Input.Initialize input) {
        Get<IGameRepo>().OnNumCoinsAtStart(input.NumCoinsInWorld);
        return this;
      }
    }
  }
}
