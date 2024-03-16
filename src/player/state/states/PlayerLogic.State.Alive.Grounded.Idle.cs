namespace GameDemo;

using Chickensoft.LogicBlocks;

public partial class PlayerLogic {
  public abstract partial record State {
    public record Idle : Grounded, IGet<Input.StartedMovingHorizontally> {
      public Idle() {
        this.OnEnter(() => Output(new Output.Animations.Idle()));
      }

      public IState On(in Input.StartedMovingHorizontally input) =>
        new Moving();
    }
  }
}
