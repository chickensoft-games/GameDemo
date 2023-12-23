namespace GameDemo;
public partial class AppLogic {
  public partial record State {
    public record LeavingGame : InGame, IGet<Input.FadeOutFinished> {
      public LeavingGame() {
        OnEnter<LeavingGame>(
          (previous) => Context.Output(new Output.FadeOut())
        );
        OnExit<LeavingGame>(
          (next) => Context.Output(new Output.RemoveExistingGame())
        );
      }

      public IState On(Input.FadeOutFinished input) {
        var appRepo = Context.Get<IAppRepo>();
        appRepo.OnGameEnded(GameOverReason.Exited);
        return this;
      }
    }
  }
}
