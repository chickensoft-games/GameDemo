namespace GameDemo;
public partial class AppLogic {
  public partial record State {
    public record PlayingGame : InGame, IGet<Input.PauseButtonPressed> {
      public PlayingGame(IContext context) : base(context) { }

      public IState On(Input.PauseButtonPressed input)
        => new GamePaused(Context);
    }
  }
}
