namespace GameDemo;

using System;
using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;

public partial record GameLogicState
{
  [Meta]
  public partial record Resuming : GameLogicState, IGet<Input.PauseMenuTransitioned>
  {
    public Resuming()
    {
      this.OnEnter(() => Get<IGameRepo>().Resume());
      this.OnExit(() => Output(new Output.HidePauseMenu()));
    }

    public Type On(in Input.PauseMenuTransitioned input) =>
      To<Playing>();
  }
}
