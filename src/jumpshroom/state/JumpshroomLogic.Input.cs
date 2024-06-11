namespace GameDemo;

public partial class JumpshroomLogic {
  public static class Input {
    public readonly record struct Hit(IPushEnabled Target);
    public readonly record struct Launch();
    public readonly record struct LaunchCompleted;
    public readonly record struct CooldownCompleted;
  }
}
