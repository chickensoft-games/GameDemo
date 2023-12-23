namespace GameDemo;

public partial class InGameUILogic {
  public interface IState : IStateLogic { }

  public record State : StateLogic, IState {
    public State() {
      OnAttach(() => {
        var appRepo = Context.Get<IAppRepo>();
        appRepo.NumCoinsCollected.Sync += OnNumCoinsCollected;
        appRepo.NumCoinsAtStart.Sync += OnNumCoinsAtStart;
      });

      OnDetach(() => {
        var appRepo = Context.Get<IAppRepo>();
        appRepo.NumCoinsCollected.Sync -= OnNumCoinsCollected;
        appRepo.NumCoinsAtStart.Sync -= OnNumCoinsAtStart;
      });
    }

    public void OnNumCoinsCollected(int numCoinsCollected) {
      Context.Output(new Output.NumCoinsCollectedChanged(numCoinsCollected));
    }

    public void OnNumCoinsAtStart(int numCoinsAtStart) {
      Context.Output(new Output.NumCoinsAtStartChanged(numCoinsAtStart));
    }
  }
}
