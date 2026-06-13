namespace GameDemo;

using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;

public partial record GameLogicState
{
  [Meta]
  public partial record Quit : GameLogicState
  {
    public Quit()
    {
      this.OnEnter(
        () => Get<IAppRepo>().OnExitGame(PostGameAction.GoToMainMenu)
      );
    }
  }
}
