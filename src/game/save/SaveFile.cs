namespace GameDemo;

using Godot;

public record CoinData(
  string NodeName,
  Transform3D Transform,
  CoinLogic.IState State
);

public record PlayerData(
  Transform3D Transform,
  Vector3 Velocity,
  PlayerLogic.IState State
);

public record MapData(
  CoinData[] Coins
);

public record GameSaveFile(
  PlayerData Player,
  MapData Map
);
