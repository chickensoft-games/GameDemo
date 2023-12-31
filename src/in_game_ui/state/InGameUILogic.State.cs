namespace GameDemo;

public partial class InGameUILogic {
  public interface IState : IStateLogic {
  }

  public record State : StateLogic, IState {
    public State() {
      OnAttach(() => {
        var gameRepo = Context.Get<IGameRepo>();
        gameRepo.NumCoinsCollected.Sync += OnNumCoinsCollected;
        gameRepo.NumCoinsAtStart.Sync += OnNumCoinsAtStart;
      });

      OnDetach(() => {
        var gameRepo = Context.Get<IGameRepo>();
        gameRepo.NumCoinsCollected.Sync -= OnNumCoinsCollected;
        gameRepo.NumCoinsAtStart.Sync -= OnNumCoinsAtStart;
      });
    }

    public void OnNumCoinsCollected(int numCoinsCollected) =>
      Context.Output(new Output.NumCoinsCollectedChanged(numCoinsCollected));

    public void OnNumCoinsAtStart(int numCoinsAtStart) =>
      Context.Output(new Output.NumCoinsAtStartChanged(numCoinsAtStart));
  }
}
