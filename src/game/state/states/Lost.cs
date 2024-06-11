namespace GameDemo;

using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;

public partial class GameLogic {
  public partial record State {
    [Meta]
    public partial record Lost : State,
    IGet<Input.Start>, IGet<Input.GoToMainMenu> {
      public Lost() {
        this.OnEnter(() => Output(new Output.ShowLostScreen()));
      }

      public Transition On(in Input.Start input) => To<RestartingGame>();
      public Transition On(in Input.GoToMainMenu input) => To<Quit>();
    }
  }
}
