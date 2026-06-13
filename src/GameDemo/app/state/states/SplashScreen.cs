namespace GameDemo;

using System;
using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;

public partial record AppLogicState
{
  [Meta]
  public partial record SplashScreen : AppLogicState, IGet<Input.FadeOutFinished>
  {
    public SplashScreen()
    {
      this.OnEnter(() => Output(new Output.ShowSplashScreen()));
    }

    public Type On(in Input.FadeOutFinished input) => To<MainMenu>();

    public void OnSplashScreenSkipped() =>
      Output(new Output.HideSplashScreen());
  }
}
