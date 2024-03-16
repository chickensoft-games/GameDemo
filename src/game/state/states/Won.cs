namespace GameDemo;

using Chickensoft.LogicBlocks;

public partial class GameLogic {
  public partial record State {
    public record Won : State, IGet<Input.GoToMainMenu> {
      public Won() {
        this.OnEnter(() => Output(new Output.ShowWonScreen()));
      }

      public IState On(in Input.GoToMainMenu input) {
        Get<IAppRepo>().OnExitGame(PostGameAction.GoToMainMenu);
        return this;
      }
    }
  }
}
