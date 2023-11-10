namespace GameDemo;

using System;

public partial class AppLogic {
  public partial record State {
    public record InGame : State,
    IGet<Input.GoToMainMenu>, IGet<Input.GameOver> {
      public InGame(IContext context) : base(context) {
        var appRepo = Context.Get<IAppRepo>();

        OnEnter<InGame>((previous) => {
          appRepo.OnStartGame();
          appRepo.GameEnded += OnGameOver;
          Context.Output(new Output.ShowGame());
        });

        OnExit<InGame>((next) => appRepo.GameEnded -= OnGameOver);
      }

      public void OnGameOver(GameOverReason reason) {
        Context.Input(new Input.GameOver(reason));
      }

      public IState On(Input.GameOver input) {
        var appRepo = Context.Get<IAppRepo>();
        appRepo.Pause();

        return input.Reason switch {
          GameOverReason.PlayerWon => new WonGame(Context),
          GameOverReason.PlayerDied => new LostGame(Context),
          GameOverReason.Exited => new MainMenu(Context),
          _ => throw new InvalidOperationException()
        };
      }

      public IState On(Input.GoToMainMenu input) => new LeavingGame(Context);
    }
  }
}
