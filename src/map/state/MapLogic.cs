namespace GameDemo;

using System;
using System.Collections.Generic;
using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;

public interface IMapLogic : ILogicBlock { }

[Meta]
public partial class MapLogic : LogicBlock, IMapLogic
{
  public override Type GetInitialState() => typeof(BaseState);

  public MapLogic()
  {
    Set(new BaseState());
  }

  public override IEnumerable<IDisposable> OnStartSubscriptions()
  {
    yield return Get<IGameRepo>().AutoChannel.Bind()
      .On((in IGameRepo.CoinCollectionStarted message) => (State as BaseState)?.OnCoinCollectionStarted(message.Coin))
      .On((in IGameRepo.CoinCollectionCompleted message) => (State as BaseState)?.OnCoinCollectionCompleted(message.Coin));
  }
}
