namespace GameDemo;

using Godot;

public abstract partial record CoinLogicState
{
  public static class Input
  {
    public readonly record struct StartCollection(ICoinCollector Target);
    public readonly record struct PhysicsProcess(
      double Delta, Vector3 GlobalPosition
    );
  }
}
