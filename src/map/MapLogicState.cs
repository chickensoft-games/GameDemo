namespace GameDemo;

using System;
using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;

[Meta, StateDiagram]
public partial record MapLogicState : LogicBlockState,
  IGet<MapLogicState.Input.GameLoadedFromSaveFile>
{
  public Type On(in Input.GameLoadedFromSaveFile input)
  {
    Get<IGameRepo>().SetNumCoinsCollected(input.NumCoinsCollected);
    return ToSelf();
  }

  public void OnCoinCollectionStarted(ICoin coin)
    => Get<MapLogic.Data>().CoinsBeingCollected.Add(coin.Name);

  public void OnCoinCollectionCompleted(ICoin coin)
  {
    Get<MapLogic.Data>().CoinsBeingCollected.Remove(coin.Name);
    Get<MapLogic.Data>().CollectedCoinIds.Add(coin.Name);
  }

  public static class Input
  {
    public readonly record struct GameLoadedFromSaveFile(int NumCoinsCollected);
  }
}
