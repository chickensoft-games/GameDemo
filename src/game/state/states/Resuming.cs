namespace GameDemo;

using Chickensoft.LogicBlocks;

public partial class GameLogic {
  public partial record State {
    public record Resuming : State, IGet<Input.PauseMenuTransitioned> {
      public Resuming() {
        this.OnEnter(() => Get<IGameRepo>().Resume());
        this.OnExit(() => Output(new Output.HidePauseMenu()));
      }

      public IState On(in Input.PauseMenuTransitioned input) =>
        new Playing();
    }
  }
}
