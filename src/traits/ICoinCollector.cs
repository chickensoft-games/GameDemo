namespace GameDemo;

using Godot;
using Chickensoft.GodotNodeInterfaces;

public interface ICoinCollector : INode {
  public Vector3 CenterOfMass { get; }
}
