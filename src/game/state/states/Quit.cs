namespace GameDemo;

using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;

public partial class GameLogic
{
  public partial record BaseState
  {
    [Meta]
    public partial record Quit : BaseState
    {
      public Quit()
      {
        this.OnEnter(
          () => Get<IAppRepo>().OnExitGame(PostGameAction.GoToMainMenu)
        );
      }
    }
  }
}
