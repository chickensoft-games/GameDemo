namespace GameDemo;

using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;

public partial class InGameUILogic
{
  [Meta]
  public partial record State : StateLogic<State>
  {
    public State()
    {
      OnAttach(() =>
      {
        var gameRepo = Get<IGameRepo>();
        gameRepo.NumCoinsCollected.Sync += OnNumCoinsCollected;
        gameRepo.NumCoinsAtStart.Sync += OnNumCoinsAtStart;
      });

      OnDetach(() =>
      {
        var gameRepo = Get<IGameRepo>();
        gameRepo.NumCoinsCollected.Sync -= OnNumCoinsCollected;
        gameRepo.NumCoinsAtStart.Sync -= OnNumCoinsAtStart;
      });
    }

    public void OnNumCoinsCollected(int numCoinsCollected) =>
      Output(new Output.NumCoinsChanged(numCoinsCollected, Get<IGameRepo>().NumCoinsAtStart.Value));

    public void OnNumCoinsAtStart(int numCoinsAtStart) =>
      Output(new Output.NumCoinsChanged(Get<IGameRepo>().NumCoinsCollected.Value, numCoinsAtStart));
  }
}
