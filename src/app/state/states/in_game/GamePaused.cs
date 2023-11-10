namespace GameDemo;
public partial class AppLogic {
  public partial record State {
    public record GamePaused : InGame, IGet<Input.PauseButtonPressed> {
      public GamePaused(IContext context) : base(context) {
        var appRepo = Context.Get<IAppRepo>();
        OnEnter<GamePaused>(
          (previous) => {
            Context.Output(new Output.ShowPauseMenu());
            appRepo.Pause();
          }
        );
        OnExit<GamePaused>(
          (next) => Context.Output(new Output.HidePauseMenu())
        );
      }

      public IState On(Input.PauseButtonPressed input)
        => new ResumingGame(Context);
    }
  }
}
