namespace GameDemo;

public partial class GameLogic {
  public partial record State {
    public record Lost : State, IGet<Input.Start>, IGet<Input.GoToMainMenu> {
      public Lost() {
        OnEnter<Lost>(
          _ => Context.Output(new Output.ShowLostScreen()));
      }

      public IState On(Input.Start input) => new RestartingGame();
      public IState On(Input.GoToMainMenu input) => new Quit();
    }
  }
}
