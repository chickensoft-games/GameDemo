namespace GameDemo;

using System;
using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;

public partial record AppLogicState
{
  [Meta]
  public partial record MainMenu : AppLogicState
    , IGet<Input.NewGame>
    , IGet<Input.LoadGame>
    , IGet<Input.DeleteGame>
  {
    public MainMenu()
    {
      this.OnEnter(() =>
      {
        Output(new Output.SetupGameScene());

        Get<IAppRepo>().OnMainMenuEntering();

        Output(new Output.ShowMainMenu());
      });
    }

    public Type On(in Input.NewGame input)
    {
      Get<AppLogic.Data>().ShouldLoadExistingGame = false;

      return To<LeavingMenu>();
    }

    public Type On(in Input.LoadGame input)
    {
      Get<AppLogic.Data>().ShouldLoadExistingGame = true;

      return To<LeavingMenu>();
    }

    public Type On(in Input.DeleteGame input)
    {
      Output(new Output.StartDeletingSaveFile());

      return ToSelf();
    }
  }
}
