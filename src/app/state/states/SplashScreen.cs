namespace GameDemo;

using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;

public partial class AppLogic
{
  public partial record State
  {
    [Meta]
    public partial record SplashScreen : State, IGet<Input.FadeOutFinished>
    {
      public SplashScreen()
      {
        this.OnEnter(() => Output(new Output.ShowSplashScreen()));

        OnAttach(
          () => Get<IAppRepo>().SplashScreenSkipped += OnSplashScreenSkipped
        );

        OnDetach(
          () => Get<IAppRepo>().SplashScreenSkipped -= OnSplashScreenSkipped
        );
      }

      public Transition On(in Input.FadeOutFinished input) => To<MainMenu>();

      public void OnSplashScreenSkipped() =>
        Output(new Output.HideSplashScreen());
    }
  }
}
