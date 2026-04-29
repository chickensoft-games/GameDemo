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
    yield return SetupSubscriptions(Get<IGameRepo>(), () => State);
  }

  public static IDisposable SetupSubscriptions(IGameRepo gameRepo, Func<LogicBlockState?> stateFunc)
  {
    return gameRepo.AutoChannel.Bind()
      .On((in IGameRepo.CoinCollectionStarted message) => stateFunc()?.Get<Data>().CoinsBeingCollected.Add(message.Coin.Name))
      .On((in IGameRepo.CoinCollectionCompleted message) =>
      {
        stateFunc()?.Get<Data>().CoinsBeingCollected.Remove(message.Coin.Name);
        stateFunc()?.Get<Data>().CollectedCoinIds.Add(message.Coin.Name);
      });
  }
}
