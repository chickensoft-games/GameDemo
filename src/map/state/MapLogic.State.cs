namespace GameDemo;

using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;
public partial class MapLogic
{
  [Meta]
  public partial record State : StateLogic<State>,
  IGet<Input.GameLoadedFromSaveFile>
  {
    public State()
    {
      OnAttach(() =>
      {
        var gameRepo = Get<IGameRepo>();
        gameRepo.CoinCollectionStarted += OnCoinCollectionStarted;
        gameRepo.CoinCollectionCompleted += OnCoinCollectionCompleted;
      });

      OnDetach(() =>
      {
        var gameRepo = Get<IGameRepo>();
        gameRepo.CoinCollectionStarted -= OnCoinCollectionStarted;
        gameRepo.CoinCollectionCompleted -= OnCoinCollectionCompleted;
      });
    }

    public Transition On(in Input.GameLoadedFromSaveFile input)
    {
      Get<IGameRepo>().SetNumCoinsCollected(input.NumCoinsCollected);
      return ToSelf();
    }

    public void OnCoinCollectionStarted(ICoin coin)
      => Get<Data>().CoinsBeingCollected.Add(coin.Name);

    public void OnCoinCollectionCompleted(ICoin coin)
    {
      Get<Data>().CoinsBeingCollected.Remove(coin.Name);
      Get<Data>().CollectedCoinIds.Add(coin.Name);
    }
  }
}
