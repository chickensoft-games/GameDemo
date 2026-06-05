namespace GameDemo;

using System;
using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;

public partial record GameLogicState
{
  [Meta]
  public partial record Lost : GameLogicState,
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
