namespace GameDemo;

using Chickensoft.LogicBlocks;

public partial class GameLogic {
  public partial record State {
    public record Lost : State, IGet<Input.Start>, IGet<Input.GoToMainMenu> {
      public Lost() {
        this.OnEnter(() => Output(new Output.ShowLostScreen()));
      }

      public IState On(in Input.Start input) => new RestartingGame();
      public IState On(in Input.GoToMainMenu input) => new Quit();
    }
  }
}
