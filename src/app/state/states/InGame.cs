namespace GameDemo;

using Chickensoft.LogicBlocks;

public partial class AppLogic {
  public partial record State {
    public record InGame : State, IGet<Input.EndGame> {
      public InGame() {
        this.OnEnter(() => {
          Get<IAppRepo>().OnEnterGame();
          Output(new Output.ShowGame());
        });
        this.OnExit(() => Output(new Output.HideGame()));

        OnAttach(() => Get<IAppRepo>().GameExited += OnGameExited);
        OnDetach(() => Get<IAppRepo>().GameExited -= OnGameExited);
      }

      public void OnRestartGameRequested() =>
        Input(new Input.EndGame(PostGameAction.RestartGame));

      public void OnGameExited(PostGameAction reason) =>
        Input(new Input.EndGame(reason));

      public IState On(in Input.EndGame input) =>
        new LeavingGame(input.PostGameAction);
    }
  }
}
