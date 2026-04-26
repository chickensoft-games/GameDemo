namespace GameDemo;

using System;
using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;

public partial class AppLogic
{
  public partial record BaseState
  {
    [Meta]
    public partial record MainMenu : BaseState,
    IGet<Input.NewGame>, IGet<Input.LoadGame>
    {
      public MainMenu()
      {
        this.OnEnter(
          () =>
          {
            Get<Data>().ShouldLoadExistingGame = false;

            Output(new Output.SetupGameScene());

            Get<IAppRepo>().OnMainMenuEntered();

            Output(new Output.ShowMainMenu());
          }
        );
      }

      public Type On(in Input.NewGame input) => To<LeavingMenu>();

      public Type On(in Input.LoadGame input)
      {
        Get<Data>().ShouldLoadExistingGame = true;

        return To<LeavingMenu>();
      }
    }
  }
}
