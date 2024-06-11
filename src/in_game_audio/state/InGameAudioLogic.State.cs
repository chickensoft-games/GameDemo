namespace GameDemo;

using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;

public partial class InGameAudioLogic {
  [Meta]
  public partial record State : StateLogic<State> {
    public State() {
      OnAttach(() => {
        var appRepo = Get<IAppRepo>();
        var gameRepo = Get<IGameRepo>();
        gameRepo.CoinCollectionStarted += OnCoinCollectionStarted;
        gameRepo.JumpshroomUsed += OnJumpshroomUsed;
        gameRepo.Ended += OnGameEnded;
        gameRepo.Jumped += OnJumped;
        appRepo.MainMenuEntered += OnMainMenuEntered;
        appRepo.GameEntered += OnGameEntered;
      });

      OnDetach(() => {
        var appRepo = Get<IAppRepo>();
        var gameRepo = Get<IGameRepo>();
        gameRepo.CoinCollectionStarted -= OnCoinCollectionStarted;
        gameRepo.JumpshroomUsed -= OnJumpshroomUsed;
        gameRepo.Ended -= OnGameEnded;
        gameRepo.Jumped -= OnJumped;
        appRepo.MainMenuEntered -= OnMainMenuEntered;
        appRepo.GameEntered -= OnGameEntered;
      });
    }

    public void OnCoinCollectionStarted(ICoin _) =>
      Output(new Output.PlayCoinCollected());

    public void OnJumpshroomUsed() => Output(new Output.PlayBounce());

    public void OnGameEnded(GameOverReason reason) {
      Output(new Output.StopGameMusic());

      if (reason is not GameOverReason.Lost) {
        return;
      }

      Output(new Output.PlayPlayerDied());
    }

    public void OnJumped() => Output(new Output.PlayJump());

    // TODO: Use a different sound system for menu sounds.

    public void OnMainMenuEntered() =>
      Output(new Output.PlayMainMenuMusic());

    public void OnGameEntered() => Output(new Output.PlayGameMusic());
  }
}
