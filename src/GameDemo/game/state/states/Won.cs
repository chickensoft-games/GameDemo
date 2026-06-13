namespace GameDemo;

using System;
using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;

public partial record GameLogicState
{
  [Meta]
  public partial record Won : GameLogicState, IGet<Input.GoToMainMenu>
  {
    public Won()
    {
      this.OnEnter(() => Output(new Output.ShowWonScreen()));
    }

    public Type On(in Input.GoToMainMenu input)
    {
      Get<IAppRepo>().OnExitGame(PostGameAction.GoToMainMenu);
      return ToSelf();
    }
  }
}
