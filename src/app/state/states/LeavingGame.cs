namespace GameDemo;

using System;
using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;

public partial class AppLogic
{
  public partial record BaseState
  {
    [Meta]
    public partial record LeavingGame : BaseState, IGet<Input.FadeOutFinished>
    {
      public LeavingGame()
      {
        this.OnExit(() => Get<Data>().PostGameAction = PostGameAction.RestartGame);
      }

      public Type On(in Input.FadeOutFinished input)
      {
        // We are either supposed to restart the game or go back to the main
        // menu. More complex games might have more post-game destinations,
        // but it's pretty simple for us.
        Output(new Output.RemoveExistingGame());

        if (Get<Data>().PostGameAction is not PostGameAction.RestartGame)
        {
          return To<MainMenu>();
        }

        Output(new Output.SetupGameScene());
        return To<InGame>();
      }
    }
  }
}
