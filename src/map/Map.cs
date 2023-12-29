namespace GameDemo;

using System.Collections.Generic;
using System.Linq;
using Chickensoft.AutoInject;
using Chickensoft.GodotNodeInterfaces;
using Chickensoft.PowerUps;
using Godot;
using SuperNodes.Types;

public interface IMap : INode3D, ISave<MapData> {
  /// <summary>Get the number of coins in the world.</summary>
  int GetCoinCount();
}

[SuperNode(typeof(AutoNode), typeof(Dependent))]
public partial class Map : Node3D, IMap {
  public override partial void _Notification(int what);

  #region Nodes
  [Node]
  public INode3D Coins { get; set; } = default!;
  #endregion Nodes

  #region Dependencies
  [Dependency]
  public IGameRepo GameRepo => DependOn<IGameRepo>();
  #endregion Dependencies

  public int GetCoinCount() => Coins.GetChildCount();

  #region Save
  public static PackedScene CoinScene => GD.Load<PackedScene>("res://src/coin/Coin.tscn");
  public string SaveId => Name;

  public MapData GetSaveData() {
    var coins = new List<CoinData>();
    foreach (var coin in Coins.GetChildrenEx().OfType<ICoin>()) {
      coins.Add(coin.GetSaveData());
    }

    return new MapData(Coins: coins.ToArray());
  }

  public void RestoreSaveData(MapData data) {
    foreach (var coinData in data.Coins) {
      // See if there's a corresponding coin that was deserialized in the map
      // scene. There always will be on a fresh deserialization, which is the only
      // time we'd be loading a map, anyways.
      var existingCoin = this.GetNodeOrNullEx<INode3D>(coinData.NodeName) as ICoin;

      if (existingCoin is not ICoin coin) {
        // Our map was deserialized without a desired coin. This will not ever happen,
        // but I'm putting this here to show how to spawn entities on load if they
        // were dynamically created during gameplay.
        coin = CoinScene.Instantiate<ICoin>();
        Coins.AddChildEx(coin);
      }

      coin.RestoreSaveData(coinData);
    }
  }

  #endregion Save
}
