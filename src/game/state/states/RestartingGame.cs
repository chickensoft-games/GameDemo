namespace GameDemo;

using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;

public partial class GameLogic
{
  public partial record BaseState
  {
    [Meta]
    public partial record RestartingGame : BaseState
    {
      public RestartingGame()
      {
        this.OnEnter(
          () => Get<IAppRepo>().OnExitGame(PostGameAction.RestartGame)
        );
      }
    }
  }
}
