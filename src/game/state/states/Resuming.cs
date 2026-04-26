namespace GameDemo;

using System;
using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;

public partial class GameLogic
{
  public partial record BaseState
  {
    [Meta]
    public partial record Resuming : BaseState, IGet<Input.PauseMenuTransitioned>
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
}
