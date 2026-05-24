namespace GameDemo;

using System;
using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;

public partial record AppLogicState
{
  [Meta]
  public partial record LeavingMenu : AppLogicState, IGet<Input.FadeOutFinished>
  {
    public LeavingMenu()
    {
      this.OnEnter(() => Output(new Output.FadeToBlack()));
    }

    public Type On(in Input.FadeOutFinished input) =>
      Get<AppLogic.Data>().ShouldLoadExistingGame
        ? To<LoadingSaveFile>()
        : To<InGame>();
  }
}
