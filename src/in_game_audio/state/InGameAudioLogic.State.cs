namespace GameDemo;

using Chickensoft.LogicBlocks;

public partial class InGameAudioLogic :
  LogicBlock<InGameAudioLogic.IState>, IInGameAudioLogic {
  public interface IState : IStateLogic {
  }

  public record State : StateLogic, IState {
    public State() {
      OnAttach(() => {
        var appRepo = Context.Get<IAppRepo>();
        var gameRepo = Context.Get<IGameRepo>();
        gameRepo.CoinCollected += OnCoinCollected;
        gameRepo.JumpshroomUsed += OnJumpshroomUsed;
        gameRepo.Ended += OnGameEnded;
        gameRepo.Jumped += OnJumped;
        appRepo.MainMenuEntered += OnMainMenuEntered;
        appRepo.GameEntered += OnGameEntered;
      });

      OnDetach(() => {
        var appRepo = Context.Get<IAppRepo>();
        var gameRepo = Context.Get<IGameRepo>();
        gameRepo.CoinCollected -= OnCoinCollected;
        gameRepo.JumpshroomUsed -= OnJumpshroomUsed;
        gameRepo.Ended -= OnGameEnded;
        gameRepo.Jumped -= OnJumped;
        appRepo.MainMenuEntered -= OnMainMenuEntered;
        appRepo.GameEntered -= OnGameEntered;
      });
    }

    public void OnCoinCollected() => Context.Output(new Output.PlayCoinCollected());

    public void OnJumpshroomUsed() => Context.Output(new Output.PlayBounce());

    public void OnGameEnded(GameOverReason reason) {
      Context.Output(new Output.StopGameMusic());

      if (reason is not GameOverReason.Lost) {
        return;
      }

      Context.Output(new Output.PlayPlayerDied());
    }

    public void OnJumped() => Context.Output(new Output.PlayJump());

    // TODO: Use a different sound system for menu sounds.

    public void OnMainMenuEntered() => Context.Output(new Output.PlayMainMenuMusic());

    public void OnGameEntered() => Context.Output(new Output.PlayGameMusic());
  }
}
