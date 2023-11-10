namespace GameDemo;

public partial class AppLogic {
  public partial record State {
    public record SplashScreen : State, IGet<Input.FadeOutFinished> {
      public SplashScreen(IContext context) : base(context) {
        var appRepo = Context.Get<IAppRepo>();
        OnEnter<SplashScreen>(
          (previous) => {
            Context.Output(new Output.ShowSplashScreen());
            appRepo.SplashScreenSkipped += OnSplashScreenSkipped;
          }
        );
        OnExit<SplashScreen>(
          (next) => appRepo.SplashScreenSkipped -= OnSplashScreenSkipped
        );
      }

      public IState On(Input.FadeOutFinished input) => new MainMenu(Context);

      public void OnSplashScreenSkipped() =>
        Context.Output(new Output.HideSplashScreen());
    }
  }
}
