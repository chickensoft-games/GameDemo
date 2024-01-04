namespace GameDemo;

public partial class AppLogic {
  public partial record State {
    public record InGame : State, IGet<Input.EndGame> {
      public InGame() {
        OnEnter<InGame>(_ => Context.Output(new Output.ShowGame()));
        OnExit<InGame>(_ => Context.Output(new Output.HideGame()));

        OnAttach(() => {
          var appRepo = Get<IAppRepo>();
          appRepo.GameEnding += OnGameEnding;
        });

        OnDetach(() => {
          var appRepo = Get<IAppRepo>();
          appRepo.GameEnding -= OnGameEnding;
        });
      }

      public void OnRestartGameRequested() =>
        Context.Input(new Input.EndGame(GameOverReason.Exited));

      public void OnGameEnding(GameOverReason reason) =>
        Context.Input(new Input.EndGame(reason));

      public IState On(Input.EndGame input) => new LeavingGame(shouldRestartGame: false);
    }
  }
}
