namespace GameDemo;

public partial class GameLogic {
  public partial record State {
    public record Lost : State, IGet<Input.StartGame> {
      public Lost() {
        OnEnter<Lost>(
          previous => Context.Output(new Output.ShowPlayerDied())
        );
      }

      public IState On(Input.StartGame input) {
        Get<IAppRepo>().RestartGame();
        return this;
      }
    }
  }
}
