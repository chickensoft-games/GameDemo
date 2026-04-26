namespace GameDemo;

using System;
using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;

public partial class AppLogic
{
  public partial record BaseState
  {
    [Meta]
    public partial record SplashScreen : BaseState, IGet<Input.FadeOutFinished>
    {
      public SplashScreen()
      {
        this.OnEnter(() => Output(new Output.ShowSplashScreen()));
      }

      public Type On(in Input.FadeOutFinished input) => To<MainMenu>();

    }
  }
}
