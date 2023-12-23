namespace GameDemo;

public partial class PlayerLogic {
  public abstract partial record State {
    public record Moving : Grounded, IGet<Input.StoppedMovingHorizontally> {
      public Moving() {
        OnEnter<Moving>(
          (previous) => Context.Output(new Output.Animations.Move())
        );
      }

      public IState On(Input.StoppedMovingHorizontally input) => new Idle();
    }
  }
}
