namespace GameDemo;

public partial class GameLogic {
  public partial record State {
    public record ResumingGame : State, IGet<Input.PauseMenuTransitioned> {
      public ResumingGame() {
        OnEnter<ResumingGame>(previous => Get<IGameRepo>().Resume());
        OnExit<ResumingGame>(
          next => Context.Output(new Output.DisablePauseMenu())
        );
      }

      public IState On(Input.PauseMenuTransitioned input) =>
        new PlayingGame();
    }
  }
}
