namespace GameDemo;

using System.Runtime.CompilerServices;
using Chickensoft.Introspection;
using Chickensoft.Serialization;
using Godot;

public partial class PlayerLogic
{
  /// <summary>Data shared between states.</summary>
  [Meta, Id("player_logic_data")]
  public partial record Data
  {
    [Save("last_strong_direction")]
    public Vector3 LastStrongDirection { get; set; } = Vector3.Forward;
    [Save("last_velocity")]
    public Vector3 LastVelocity { get; set; } = Vector3.Zero;
    [Save("was_on_floor")]
    public bool WasOnFloor { get; set; } = true;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool HadNegativeYVelocity() => LastVelocity.Y < 0f;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool WasMovingHorizontally(Settings settings) =>
      (LastVelocity with { Y = 0f }).Length() >= settings.StoppingSpeed;
  }
}
