namespace GameDemo;

using System.Linq;
using Chickensoft.AutoInject;
using Chickensoft.Collections;
using Chickensoft.GodotNodeInterfaces;
using Chickensoft.Introspection;
using Chickensoft.SaveFileBuilder;
using Godot;

public interface IMap : INode3D,
IProvide<ISaveChunk<MapData>>, IProvide<EntityTable>
{
  /// <summary>Get the number of coins in the world.</summary>
  int GetCoinCount();
}

[Meta(typeof(IAutoNode))]
public partial class Map : Node3D, IMap
{
  public override void _Notification(int what) => this.Notify(what);

  #region Save

  [Dependency]
  public EntityTable EntityTable => this.DependOn<EntityTable>();
  EntityTable IProvide<EntityTable>.Value() => EntityTable;
  [Dependency]
  public ISaveChunk<GameData> GameChunk => this.DependOn<ISaveChunk<GameData>>();
  public ISaveChunk<MapData> MapChunk { get; set; } = default!;
  ISaveChunk<MapData> IProvide<ISaveChunk<MapData>>.Value() => MapChunk;

  #endregion Save

  #region Nodes

  [Node] public INode3D Coins { get; set; } = default!;

  #endregion Nodes

  #region State

  [Dependency] public IGameRepo GameRepo => this.DependOn<IGameRepo>();
  public IMapLogic MapLogic { get; set; } = default!;

  #endregion State

  public int GetCoinCount() => Coins.GetChildCount();

  public void Setup() => MapLogic = new MapLogic();

  public void OnResolved()
  {
    MapChunk = new SaveChunk<MapData>(
      onSave: (chunk) =>
      {
        var data = MapLogic.Get<MapLogic.Data>();

        // Based on the data we track in our state, construct a map data
        // object with information about the coins being collected and the
        // ones already collected (so we can remove them when we load a save
        // file).
        return new MapData()
        {
          CoinsBeingCollected = data.CoinsBeingCollected.ToDictionary(
            keySelector: (coinName) => coinName,
            elementSelector: coinName =>
            {
              var coin = EntityTable.Get<ICoin>(coinName)!;
              return new CoinData()
              {
                StateMachine = coin.CoinLogic,
                GlobalTransform = coin.GlobalTransform
              };
            }
          ),
          CollectedCoinIds = data.CollectedCoinIds
        };
      },
      onLoad: (chunk, data) =>
      {
        // Remove previously collected coins from the fresh map.
        foreach (var coinId in data.CollectedCoinIds)
        {
          if (Coins.GetNodeOrNullEx<INode>(coinId) is { } coin)
          {
            Coins.RemoveChildEx(coin);
            coin.QueueFree();
          }
        }

        // Restore the coins actively being collected.
        foreach ((var coinName, var coinData) in data.CoinsBeingCollected)
        {
          var child = Coins.GetNodeOrNullEx<INode>(coinName);
          if (child is ICoin coin)
          {
            coin.CoinLogic.RestoreFrom(coinData.StateMachine);
            coin.CoinLogic.Start();
            coin.GlobalTransform = coinData.GlobalTransform;
          }
        }

        // Update our state blackboard data with the loaded data.
        var mapLogicData = MapLogic.Get<MapLogic.Data>();

        mapLogicData.CollectedCoinIds = data.CollectedCoinIds;

        mapLogicData.CoinsBeingCollected =
          [.. data.CoinsBeingCollected.Keys];

        var numCoinsCollected = data.CollectedCoinIds.Count +
          data.CoinsBeingCollected.Count;

        MapLogic.Input(
          new MapLogic.Input.GameLoadedFromSaveFile(
            NumCoinsCollected: numCoinsCollected
          )
        );
      }
    );

    // Add a child to our parent save chunk (the game chunk) so that it can
    // look up the map chunk when loading and saving the game.
    GameChunk.AddChunk(MapChunk);

    MapLogic.Set(new MapLogic.Data());
    MapLogic.Set(GameRepo);

    MapLogic.Start();

    this.Provide();
  }
}
