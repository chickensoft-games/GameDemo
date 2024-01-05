namespace GameDemo;

public partial class PlayerLogic {
  public abstract partial record State : StateLogic, IState {
    public record Disabled : State, IGet<Input.Enable> {
      public Disabled() {
        OnEnter<Disabled>(
          previous => Context.Output(new Output.Animations.Idle())
        );

        OnAttach(() => Get<IAppRepo>().GameEntered += OnGameAboutToStart);
        OnDetach(() => Get<IAppRepo>().GameEntered -= OnGameAboutToStart);
      }

      public IState On(Input.Enable input) => new Idle();
    }

    public void OnGameAboutToStart() => Context.Input(new Input.Enable());
  }
}
