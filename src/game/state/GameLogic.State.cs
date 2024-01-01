namespace GameDemo;

public partial class GameLogic {
  public interface IState : IStateLogic {
  }

  public abstract partial record State : StateLogic, IState, IGet<Input.Initialize> {
    protected State() {
      OnAttach(() => Get<IGameRepo>().IsMouseCaptured.Sync += OnIsMouseCaptured);
      OnDetach(() => Get<IGameRepo>().IsMouseCaptured.Sync -= OnIsMouseCaptured);
    }

    private void OnIsMouseCaptured(bool isMouseCaptured) =>
      Context.Output(new Output.CaptureMouse(isMouseCaptured));

    public IState On(Input.Initialize input) {
      Get<IGameRepo>().OnNumCoinsAtStart(input.NumCoinsInWorld);
      return this;
    }
  }
}
