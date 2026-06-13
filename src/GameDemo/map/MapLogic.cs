namespace GameDemo;

using System;
using System.Collections.Generic;
using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;
using Chickensoft.LogicBlocks.Auto;

public interface IMapLogic : ILogicBlock { }

[Meta]
public partial class MapLogic : AutoBlock, IMapLogic
{
  public MapLogic()
  {
    Preallocate<MapLogicState>();
  }

  public override IEnumerable<IDisposable> OnStartSubscriptions()
  {
    yield return Get<IGameRepo>().AutoChannel.Bind()
      .On((in IGameRepo.CoinCollectionStarted message) => (State as MapLogicState)?.OnCoinCollectionStarted(message.Coin))
      .On((in IGameRepo.CoinCollectionCompleted message) => (State as MapLogicState)?.OnCoinCollectionCompleted(message.Coin));
  }
}
