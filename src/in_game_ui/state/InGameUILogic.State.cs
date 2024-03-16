namespace GameDemo;

using Chickensoft.LogicBlocks;

public partial class InGameUILogic {
  public interface IState : IStateLogic<IState> {
  }

  public record State : StateLogic<IState>, IState {
    public State() {
      OnAttach(() => {
        var gameRepo = Get<IGameRepo>();
        gameRepo.NumCoinsCollected.Sync += OnNumCoinsCollected;
        gameRepo.NumCoinsAtStart.Sync += OnNumCoinsAtStart;
      });

      OnDetach(() => {
        var gameRepo = Get<IGameRepo>();
        gameRepo.NumCoinsCollected.Sync -= OnNumCoinsCollected;
        gameRepo.NumCoinsAtStart.Sync -= OnNumCoinsAtStart;
      });
    }

    public void OnNumCoinsCollected(int numCoinsCollected) =>
      Output(new Output.NumCoinsCollectedChanged(numCoinsCollected));

    public void OnNumCoinsAtStart(int numCoinsAtStart) =>
      Output(new Output.NumCoinsAtStartChanged(numCoinsAtStart));
  }
}
