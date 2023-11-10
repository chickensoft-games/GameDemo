namespace GameDemo;

using Godot;

/// <summary>
/// Represents an object that can have a force applied to it.
/// </summary>
public interface IPushEnabled {
  /// <summary>
  /// Applies a force to the object.
  /// </summary>
  /// <param name="force">Global force impulse vector to apply.</param>
  void Push(Vector3 force);
}
