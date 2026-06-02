namespace GameDemo;

using Godot;

public abstract partial record CoinLogicState
{
  public static class Output
  {
    public readonly record struct Move(Vector3 GlobalPosition);
    public readonly record struct SelfDestruct();
  }
}
