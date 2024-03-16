namespace GameDemo;

using Chickensoft.LogicBlocks;

public partial class PlayerLogic {
  public abstract partial record State {
    public record Moving : Grounded, IGet<Input.StoppedMovingHorizontally> {
      public Moving() {
        this.OnEnter(() => Output(new Output.Animations.Move()));
      }

      public IState On(in Input.StoppedMovingHorizontally input) => new Idle();
    }
  }
}
