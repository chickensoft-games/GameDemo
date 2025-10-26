namespace GameDemo;

using Chickensoft.Introspection;
using Chickensoft.Serialization;
using Godot;

[Meta, Id("player_data")]
public partial record PlayerData
{
  [Save("global_transform")]
  public required Transform3D GlobalTransform { get; init; }
  [Save("state_machine")]
  public required IPlayerLogic StateMachine { get; init; }
  [Save("velocity")]
  public required Vector3 Velocity { get; init; }
}
