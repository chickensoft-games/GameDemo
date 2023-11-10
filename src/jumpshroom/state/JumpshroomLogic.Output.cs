namespace GameDemo;

using Chickensoft.LogicBlocks;

public partial class JumpshroomLogic : LogicBlock<JumpshroomLogic.IState> {
  public static class Output {
    public readonly record struct Animate;
    public readonly record struct StartCooldownTimer;
  }
}
