namespace GameDemo;

public partial class PlayerLogic {
  public abstract partial record State {
    public record Airborne : Alive,
    IGet<Input.HitFloor>, IGet<Input.StartedFalling> {
      public Airborne(IContext context) : base(context) { }

      public IState On(Input.HitFloor input) {
        if (input.IsMovingHorizontally) {
          return new Moving(Context);
        }
        return new Idle(Context);
      }

      public IState On(Input.StartedFalling input) => new Falling(Context);
    }
  }
}
