namespace GameDemo;
public partial class AppLogic {
  public interface IState : IStateLogic { }

  public partial record State : StateLogic, IState {
    public State(IContext context) : base(context) {
      var appRepo = Context.Get<IAppRepo>();
      OnEnter<State>(
        (previous) => appRepo.IsMouseCaptured.Sync += OnMouseCaptured
      );
      OnExit<State>((next) => appRepo.IsMouseCaptured.Sync -= OnMouseCaptured);
    }

    public void OnMouseCaptured(bool isMouseCaptured) =>
      Context.Output(new Output.CaptureMouse(isMouseCaptured));
  }
}
