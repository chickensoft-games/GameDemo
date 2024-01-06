namespace GameDemo;

partial class PlayerLogic {
  partial record State {
    public record Falling : Airborne {
      public Falling() {
        OnEnter<Falling>(
          previous => Context.Output(new Output.Animations.Fall())
        );
      }
    }
  }
}
