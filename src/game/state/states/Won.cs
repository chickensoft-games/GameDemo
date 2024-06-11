namespace GameDemo;

using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;

public partial class GameLogic {
  public partial record State {
    [Meta]
    public partial record Won : State, IGet<Input.GoToMainMenu> {
      public Won() {
        this.OnEnter(() => Output(new Output.ShowWonScreen()));
      }

      public Transition On(in Input.GoToMainMenu input) {
        Get<IAppRepo>().OnExitGame(PostGameAction.GoToMainMenu);
        return ToSelf();
      }
    }
  }
}
