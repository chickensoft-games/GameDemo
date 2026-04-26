namespace GameDemo;

using System;
using System.Collections.Generic;
using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;

public interface IMapLogic : ILogicBlock { }

[Meta]
public partial class MapLogic : LogicBlock, IMapLogic
{
  public override Type GetInitialState() => typeof(State);

  public MapLogic()
  {
    Set(new State());
  }

  public override IEnumerable<IDisposable> OnStartSubscriptions()
  {
    yield return Get<IGameRepo>().AutoChannel.Bind()
      .On((in IGameRepo.CoinCollectionStarted message) => Get<Data>().CoinsBeingCollected.Add(message.Coin.Name))
      .On((in IGameRepo.CoinCollectionCompleted message) =>
      {
        Get<Data>().CoinsBeingCollected.Remove(message.Coin.Name);
        Get<Data>().CollectedCoinIds.Add(message.Coin.Name);
      });
  }
}
