namespace GameDemo;

using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;

public partial class AppLogic {
  public partial record State {
    [Meta]
    public partial record InGame : State, IGet<Input.EndGame> {
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

      public Transition On(in Input.EndGame input) {
        var postGameAction = input.PostGameAction;
        return To<LeavingGame>().With(
          (state) => ((LeavingGame)state).PostGameAction = postGameAction
        );
      }
    }
  }
}
