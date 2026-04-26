namespace GameDemo;

using System;
using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;

public partial class GameLogic
{
  public partial record BaseState
  {
    [Meta]
    public partial record Lost : BaseState,
    IGet<Input.Start>, IGet<Input.GoToMainMenu>
    {
      public Lost()
      {
        this.OnEnter(() => Output(new Output.ShowLostScreen()));
      }

      public Type On(in Input.Start input) => To<RestartingGame>();
      public Type On(in Input.GoToMainMenu input) => To<Quit>();
    }
  }
}
