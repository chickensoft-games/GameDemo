namespace GameDemo;

public partial class GameLogic {
  public static class Output {
    public readonly record struct CaptureMouse(bool IsMouseCaptured);

    public readonly record struct ChangeToThirdPersonCamera;

    public readonly record struct SetPauseMode(bool IsPaused);

    public readonly record struct ShowPauseSaveOverlay;

    public readonly record struct HidePauseSaveOverlay;

    public readonly record struct DisablePauseMenu;

    public readonly record struct ShowPauseMenu;

    public readonly record struct HidePauseMenu;

    public readonly record struct ShowPlayerDied;

    public readonly record struct ShowPlayerWon;
  }
}
