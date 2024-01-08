namespace GameDemo;

public partial class AppLogic {
  public partial record State {
    public record InGame : State, IGet<Input.EndGame> {
      public InGame() {
        OnEnter<InGame>(_ => {
          Get<IAppRepo>().OnEnterGame();
          Context.Output(new Output.ShowGame());
        });
        OnExit<InGame>(_ => Context.Output(new Output.HideGame()));

        OnAttach(() => Get<IAppRepo>().GameExited += OnGameExited);
        OnDetach(() => Get<IAppRepo>().GameExited -= OnGameExited);
      }

      public void OnRestartGameRequested() =>
        Context.Input(new Input.EndGame(PostGameAction.RestartGame));

      public void OnGameExited(PostGameAction reason) =>
        Context.Input(new Input.EndGame(reason));

      public IState On(Input.EndGame input) =>
        new LeavingGame(input.PostGameAction);
    }
  }
}
