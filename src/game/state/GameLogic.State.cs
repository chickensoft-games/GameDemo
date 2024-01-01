namespace GameDemo;

public partial class GameLogic {
  public interface IState : IStateLogic {
  }

  public abstract partial record State : StateLogic, IState {
    protected State() {
      OnAttach(() => Get<IGameRepo>().IsMouseCaptured.Sync += OnIsMouseCaptured);
      OnDetach(() => Get<IGameRepo>().IsMouseCaptured.Sync -= OnIsMouseCaptured);
    }

    private void OnIsMouseCaptured(bool isMouseCaptured) =>
      Context.Output(new Output.CaptureMouse(isMouseCaptured));
  }
}
