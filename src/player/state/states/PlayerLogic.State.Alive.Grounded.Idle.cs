namespace GameDemo;

public partial class PlayerLogic {
  public abstract partial record State {
    public record Idle : Grounded, IGet<Input.StartedMovingHorizontally> {
      public Idle() {
        OnEnter<Idle>(
          (previous) => Context.Output(new Output.Animations.Idle())
        );
      }

      public IState On(Input.StartedMovingHorizontally input) => new Moving();
    }
  }
}
