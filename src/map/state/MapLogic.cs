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
  public override Type GetInitialState() => typeof(BaseState);

  public MapLogic()
  {
    Preallocate<BaseState>();
  }

  public override IEnumerable<IDisposable> OnStartSubscriptions()
  {
    yield return Get<IGameRepo>().AutoChannel.Bind()
      .On((in IGameRepo.CoinCollectionStarted message) => (State as BaseState)?.OnCoinCollectionStarted(message.Coin))
      .On((in IGameRepo.CoinCollectionCompleted message) => (State as BaseState)?.OnCoinCollectionCompleted(message.Coin));
  }
}
