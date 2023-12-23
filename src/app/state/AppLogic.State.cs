namespace GameDemo;
public partial class AppLogic {
  public interface IState : IStateLogic { }

  public partial record State : StateLogic, IState {
    public State() {
      OnAttach(
        () => Get<IAppRepo>().IsMouseCaptured.Sync += OnMouseCaptured
      );
      OnDetach(() => Get<IAppRepo>().IsMouseCaptured.Sync -= OnMouseCaptured);
    }

    public void OnMouseCaptured(bool isMouseCaptured) =>
      Context.Output(new Output.CaptureMouse(isMouseCaptured));
  }
}
