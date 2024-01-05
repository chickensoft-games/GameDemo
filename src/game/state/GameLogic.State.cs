namespace GameDemo;

public partial class GameLogic {
  public interface IState : IStateLogic {
  }

  public abstract partial record State : StateLogic, IState {
    protected State() {
      OnAttach(() => {
        var gameRepo = Get<IGameRepo>();
        gameRepo.IsMouseCaptured.Sync += OnIsMouseCaptured;
        gameRepo.IsPaused.Sync += OnIsPaused;
      });
      OnDetach(() => {
        var gameRepo = Get<IGameRepo>();
        gameRepo.IsMouseCaptured.Sync -= OnIsMouseCaptured;
        gameRepo.IsPaused.Sync -= OnIsPaused;
      });
    }

    private void OnIsMouseCaptured(bool isMouseCaptured) =>
      Context.Output(new Output.CaptureMouse(isMouseCaptured));

    private void OnIsPaused(bool isPaused) =>
      Context.Output(new Output.SetPauseMode(isPaused));
  }
}
