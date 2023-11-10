namespace GameDemo;

public partial class PlayerLogic {
  public abstract partial record State : StateLogic, IState {
    public record Disabled : State, IGet<Input.Enable> {
      public Disabled(IContext context) : base(context) {
        var appRepo = Context.Get<IAppRepo>();

        OnEnter<Disabled>(
          (previous) => {
            Context.Output(new Output.Animations.Idle());
            appRepo.GameStarting += OnGameAboutToStart;
          }
        );

        OnExit<Disabled>(
          (next) => appRepo.GameStarting -= OnGameAboutToStart
        );
      }

      public IState On(Input.Enable input) => new Idle(Context);
    }

    public void OnGameAboutToStart() => Context.Input(new Input.Enable());
  }
}
