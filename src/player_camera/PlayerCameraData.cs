namespace GameDemo;

using Chickensoft.Introspection;
using Chickensoft.Serialization;
using Godot;

[Meta, Id("player_camera_data")]
public partial record PlayerCameraData {
  [Save("state_machine")]
  public required PlayerCameraLogic StateMachine { get; init; }

  [Save("global_transform")]
  public required Transform3D GlobalTransform { get; init; }

  [Save("local_position")]
  public required Vector3 LocalPosition { get; init; }

  [Save("offset_position")]
  public required Vector3 OffsetPosition { get; init; }
}
