namespace GameDemo;

using System;
using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;

public partial record AppLogicState
{
  [Meta]
  public partial record MainMenu : AppLogicState,
    IGet<Input.NewGame>, IGet<Input.LoadGame>
  {
    public MainMenu()
    {
      this.OnEnter(
        () =>
        {
          Get<AppLogic.Data>().ShouldLoadExistingGame = false;

          Output(new Output.SetupGameScene());

          Get<IAppRepo>().OnMainMenuEntered();

          Output(new Output.ShowMainMenu());
        }
      );
    }

    public Type On(in Input.NewGame input) => To<LeavingMenu>();

    public Type On(in Input.LoadGame input)
    {
      Get<AppLogic.Data>().ShouldLoadExistingGame = true;

      return To<LeavingMenu>();
    }
  }
}
