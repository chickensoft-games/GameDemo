namespace GameDemo;

using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;

public partial record GameLogicState
{
  [Meta]
  public partial record RestartingGame : GameLogicState
  {
    public RestartingGame()
    {
      this.OnEnter(
        () => Get<IAppRepo>().OnExitGame(PostGameAction.RestartGame)
      );
    }
  }
}
