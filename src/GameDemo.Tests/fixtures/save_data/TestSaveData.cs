namespace GameDemo.Tests;

using Godot;

public static class TestSaveData
{
  public static MapData MapData { get; } = new()
  {
    CoinsBeingCollected = [],
    CollectedCoinIds = []
  };

  public static PlayerData PlayerData { get; } = new()
  {
    GlobalTransform = Transform3D.Identity,
    StateMachine = null!,
    Velocity = Vector3.Zero
  };

  public static PlayerCameraData PlayerCameraData { get; } = new()
  {
    GlobalTransform = Transform3D.Identity,
    StateMachine = null!,
    LocalPosition = Vector3.Zero,
    OffsetPosition = Vector3.Zero
  };

  public static GameData GameData { get; } = new()
  {
    MapData = null!,
    PlayerData = null!,
    PlayerCameraData = null!
  };

  static TestSaveData()
  {
  }
}
