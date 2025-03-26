namespace GameDemo;

using Chickensoft.GodotNodeInterfaces;
using Godot;

public interface ICoinCollector : INode {
  Vector3 CenterOfMass { get; }
}
