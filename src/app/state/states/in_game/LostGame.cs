namespace GameDemo;
public partial class AppLogic {
  public partial record State {
    public record LostGame : InGame, IGet<Input.StartGame> {
      public LostGame() {
        OnEnter<LostGame>(
          (previous) => Context.Output(new Output.ShowPlayerDied())
        );
      }

      public IState On(Input.StartGame input) =>
        new RestartingGame();
    }
  }
}
