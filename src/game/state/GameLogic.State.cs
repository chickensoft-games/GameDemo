namespace GameDemo;

public partial class GameLogic {
  public interface IState : IStateLogic {
  }

  public partial record State : StateLogic, IState, IGet<Input.Initialize> {
    public State() {
      OnAttach(
        () => {
          var appRepo = Get<IAppRepo>();
          appRepo.GameStarting += GameAboutToStart;

          var gameRepo = Get<IGameRepo>();
          gameRepo.GamePaused += OnGamePaused;
          gameRepo.GameResumed += OnGameResumed;
        }
      );

      OnDetach(() => {
        var appRepo = Context.Get<IAppRepo>();
        appRepo.GameStarting -= GameAboutToStart;

        var gameRepo = Get<IGameRepo>();
        gameRepo.GamePaused -= OnGamePaused;
        gameRepo.GameResumed -= OnGameResumed;
      });
    }

    public void GameAboutToStart() =>
      Context.Output(new Output.ChangeToThirdPersonCamera());

    public void OnGamePaused() => Context.Output(new Output.SetPauseMode(true));
    public void OnGameResumed() => Context.Output(new Output.SetPauseMode(false));

    public IState On(Input.Initialize input) {
      Get<IGameRepo>().OnNumCoinsAtStart(input.NumCoinsInWorld);
      return this;
    }
  }
}
