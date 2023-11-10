namespace GameDemo;

public partial class PlayerLogic {
  public abstract partial record State {
    public record Falling : Airborne {
      public Falling(IContext context) : base(context) {
        OnEnter<Falling>(
          (previous) => Context.Output(new Output.Animations.Fall())
        );
      }
    }
  }
}
