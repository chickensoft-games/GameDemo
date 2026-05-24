namespace GameDemo;

using System;
using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;

public partial record AppLogicState
{
  [Meta]
  public partial record InGame : AppLogicState, IGet<Input.EndGame>
  {
    public InGame()
    {
      this.OnEnter(() =>
      {
        Get<IAppRepo>().OnEnterGame();
        Output(new Output.ShowGame());
      });
      this.OnExit(() => Output(new Output.HideGame()));
    }

    public void OnRestartGameRequested() =>
      Input(new Input.EndGame(PostGameAction.RestartGame));

    public void OnGameExited(PostGameAction reason) =>
      Input(new Input.EndGame(reason));

    public Type On(in Input.EndGame input)
    {
      var postGameAction = input.PostGameAction;
      Get<AppLogic.Data>().PostGameAction = postGameAction;
      return To<LeavingGame>();
    }
  }
}
