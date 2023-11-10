namespace GameDemo;

using Godot;

public partial class PlayerLogic {
  public static class Output {
    public static class Animations {
      public readonly record struct Idle;
      public readonly record struct Move;
      public readonly record struct Jump;
      public readonly record struct Fall;
    }
    public readonly record struct MoveSpeedChanged(float Speed);
    public readonly record struct MovementComputed(
      Basis Rotation, Vector3 Velocity
    );
    /// <summary>Output when the player has just come to a stop.</summary>
    public readonly record struct Stopped;
    public readonly record struct VelocityChanged(Vector3 Velocity);
  }
}
