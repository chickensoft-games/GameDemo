namespace GameDemo;

public partial class AppLogic {
  public partial record State {
    public record InGame : State, IGet<Input.StartGame>, IGet<Input.EndGame> {
      public InGame() {
        OnEnter<InGame>(_ => Context.Output(new Output.ShowGame()));
        OnExit<InGame>(_ => Context.Output(new Output.HideGame()));

        OnAttach(() => {
          var appRepo = Get<IAppRepo>();
          appRepo.RestartGameRequested += OnRestartGameRequested;
          appRepo.GameEnding += OnGameEnding;
        });

        OnDetach(() => {
          Get<IAppRepo>().RestartGameRequested -= OnRestartGameRequested;
        });
      }

      public void OnRestartGameRequested() =>
        Context.Input(new Input.StartGame());

      public IState On(Input.StartGame input) => new LeavingGame();

      public void OnGameEnding(GameOverReason reason) =>
        Context.Input(new Input.EndGame(reason));

      public IState On(Input.EndGame input) => new LeavingGame();
    }
  }
}
