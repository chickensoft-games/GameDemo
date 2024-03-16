namespace GameDemo;

using Chickensoft.LogicBlocks;

public partial class PlayerLogic {
  public partial record State {
    public record Falling : Airborne {
      public Falling() {
        this.OnEnter(() => Output(new Output.Animations.Fall()));
      }
    }
  }
}
