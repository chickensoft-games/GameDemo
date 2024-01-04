namespace GameDemo;

public partial class GameLogic {
  public partial record State {
    public record Lost : State, IGet<Input.StartGame> {
      public Lost() {
        OnEnter<Lost>(
          _ => Context.Output(new Output.ShowPlayerDied())
        );
      }

      public IState On(Input.StartGame input) {
        Get<IAppRepo>().EndGame(GameOverReason.Exited);
        return this;
      }
    }
  }
}
