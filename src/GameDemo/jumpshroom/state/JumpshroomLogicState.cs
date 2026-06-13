namespace GameDemo;

using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;

[Meta, StateDiagram]
public abstract partial record JumpshroomLogicState : LogicBlockState
{
  public static class Input
  {
    public readonly record struct Hit(IPushEnabled Target);
    public readonly record struct Launch();
    public readonly record struct LaunchCompleted;
    public readonly record struct CooldownCompleted;
  }

  public static class Output
  {
    public readonly record struct Animate;
    public readonly record struct StartCooldownTimer;
  }
}
