namespace GameDemo;

using Chickensoft.Introspection;
using Chickensoft.Serialization;
using Godot;


[Meta, Id("coin_data")]
public partial record CoinData {
  [Save("state_machine")]
  public required ICoinLogic StateMachine { get; init; }
  [Save("global_transform")]
  public required Transform3D GlobalTransform { get; init; }
}
