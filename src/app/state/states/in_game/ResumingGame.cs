namespace GameDemo;
public partial class AppLogic {
  public partial record State {
    public record ResumingGame : InGame, IGet<Input.PauseMenuTransitioned> {
      public ResumingGame(IContext context) : base(context) {
        var appRepo = Context.Get<IAppRepo>();
        OnEnter<ResumingGame>((previous) => appRepo.Resume());
        OnExit<ResumingGame>(
          (next) => Context.Output(new Output.DisablePauseMenu())
        );
      }

      public IState On(Input.PauseMenuTransitioned input) =>
        new PlayingGame(Context);
    }
  }
}
