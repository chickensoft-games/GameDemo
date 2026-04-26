namespace GameDemo;

using System;
using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;

public partial class AppLogic
{
  public partial record BaseState
  {
    [Meta]
    public partial record InGame : BaseState, IGet<Input.EndGame>
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

      public Type On(in Input.EndGame input)
      {
        var postGameAction = input.PostGameAction;
        Get<Data>().PostGameAction = postGameAction;
        return To<LeavingGame>();
      }
    }
  }
}
