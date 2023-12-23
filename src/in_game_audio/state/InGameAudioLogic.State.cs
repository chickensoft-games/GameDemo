namespace GameDemo;

using Chickensoft.LogicBlocks;

public partial class InGameAudioLogic :
  LogicBlock<InGameAudioLogic.IState>, IInGameAudioLogic {
  public interface IState : IStateLogic { }

  public record State : StateLogic, IState {
    public State() {
      OnAttach(() => {
        var appRepo = Context.Get<IAppRepo>();
        appRepo.CoinCollected += OnCoinCollected;
        appRepo.JumpshroomUsed += OnJumpshroomUsed;
        appRepo.GameEnded += OnGameEnded;
        appRepo.Jumped += OnJumped;
        appRepo.MainMenuEntered += OnMainMenuEntered;
        appRepo.GameStarting += OnGameStarting;
      });

      OnDetach(() => {
        var appRepo = Context.Get<IAppRepo>();
        appRepo.CoinCollected -= OnCoinCollected;
        appRepo.JumpshroomUsed -= OnJumpshroomUsed;
        appRepo.GameEnded -= OnGameEnded;
        appRepo.Jumped -= OnJumped;
        appRepo.MainMenuEntered -= OnMainMenuEntered;
        appRepo.GameStarting -= OnGameStarting;
      });
    }

    public void OnCoinCollected() {
      Context.Output(new Output.PlayCoinCollected());
    }

    public void OnJumpshroomUsed() {
      Context.Output(new Output.PlayBounce());
    }

    public void OnGameEnded(GameOverReason reason) {
      if (reason == GameOverReason.PlayerDied) {
        Context.Output(new Output.PlayPlayerDied());
      }
    }

    public void OnJumped() {
      Context.Output(new Output.PlayJump());
    }

    public void OnMainMenuEntered() {
      Context.Output(new Output.PlayMainMenuMusic());
    }

    public void OnGameStarting() {
      Context.Output(new Output.PlayGameMusic());
    }
  }
}
