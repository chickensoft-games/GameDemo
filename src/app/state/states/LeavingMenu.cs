namespace GameDemo;

using Chickensoft.LogicBlocks;

public partial class AppLogic {
  public partial record State {
    public record LeavingMenu : State, IGet<Input.FadeOutFinished> {
      public LeavingMenu() {
        this.OnEnter(() => Output(new Output.FadeToBlack()));
      }

      public IState On(in Input.FadeOutFinished input) => new InGame();
    }
  }
}
