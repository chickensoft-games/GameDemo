namespace GameDemo;

using Chickensoft.LogicBlocks;

public partial class JumpshroomLogic : LogicBlock<JumpshroomLogic.IState> {
  public static class Input {
    public readonly record struct Hit(IPushEnabled Target);
    public readonly record struct Launch();
    public readonly record struct LaunchCompleted;
    public readonly record struct CooldownCompleted;
  }
}
