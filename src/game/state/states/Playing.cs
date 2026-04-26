namespace GameDemo;

using System;
using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;

public partial class GameLogic
{
  public partial record BaseState
  {
    [Meta]
    public partial record Playing : BaseState,
    IGet<Input.EndGame>, IGet<Input.PauseButtonPressed>
    {
      public Playing()
      {
        this.OnEnter(
          () =>
          {
            Output(new Output.StartGame());
            Get<IGameRepo>().SetIsMouseCaptured(true);
          }
        );
      }

      public Type On(in Input.EndGame input)
      {
        Get<IGameRepo>().Pause();

        return input.Reason switch
        {
          GameOverReason.Won => To<Won>(),
          GameOverReason.Lost => To<Lost>(),
          GameOverReason.Quit => To<Quit>(),
          _ => To<Quit>()
        };
      }

      public Type On(in Input.PauseButtonPressed input) => To<Paused>();
    }
  }
}
