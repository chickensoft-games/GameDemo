namespace GameDemo;

using Chickensoft.LogicBlocks;

public partial class AppLogic {
  public partial record State {
    public record SplashScreen : State, IGet<Input.FadeOutFinished> {
      public SplashScreen() {
        this.OnEnter(() => Output(new Output.ShowSplashScreen()));

        OnAttach(
          () => Get<IAppRepo>().SplashScreenSkipped += OnSplashScreenSkipped
        );

        OnDetach(
          () => Get<IAppRepo>().SplashScreenSkipped -= OnSplashScreenSkipped
        );
      }

      public IState On(in Input.FadeOutFinished input) => new MainMenu();

      public void OnSplashScreenSkipped() =>
        Output(new Output.HideSplashScreen());
    }
  }
}
