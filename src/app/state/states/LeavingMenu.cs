namespace GameDemo;

using System;
using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;

public partial class AppLogic
{
  public partial record BaseState
  {
    [Meta]
    public partial record LeavingMenu : BaseState, IGet<Input.FadeOutFinished>
    {
      public LeavingMenu()
      {
        this.OnEnter(() => Output(new Output.FadeToBlack()));
      }

      public Type On(in Input.FadeOutFinished input) =>
        Get<Data>().ShouldLoadExistingGame
          ? To<LoadingSaveFile>()
          : To<InGame>();
    }
  }
}
