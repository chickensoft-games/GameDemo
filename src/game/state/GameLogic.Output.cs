namespace GameDemo;

public partial class GameLogic {
  public static class Output {
    public readonly record struct MouseCaptured(bool IsMouseCaptured);
    public readonly record struct ChangeToThirdPersonCamera;
    public readonly record struct SetPauseMode(bool IsPaused);
  }
}
