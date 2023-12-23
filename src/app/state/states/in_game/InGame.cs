namespace GameDemo;

using System;

public partial class AppLogic {
  public partial record State {
    public record InGame : State,
    IGet<Input.GoToMainMenu>, IGet<Input.GameOver> {
      public InGame() {
        OnEnter<InGame>((previous) => {
          Get<IAppRepo>().OnStartGame();
          Context.Output(new Output.ShowGame());
        });

        OnAttach(() => Get<IAppRepo>().GameEnded += OnGameOver);
        OnDetach(() => Get<IAppRepo>().GameEnded -= OnGameOver);
      }

      public void OnGameOver(GameOverReason reason) {
        Context.Input(new Input.GameOver(reason));
      }

      public IState On(Input.GameOver input) {
        var appRepo = Context.Get<IAppRepo>();
        appRepo.Pause();

        return input.Reason switch {
          GameOverReason.PlayerWon => new WonGame(),
          GameOverReason.PlayerDied => new LostGame(),
          GameOverReason.Exited => new MainMenu(),
          _ => throw new InvalidOperationException()
        };
      }

      public IState On(Input.GoToMainMenu input) => new LeavingGame();
    }
  }
}
