namespace GameDemo;

public partial class PlayerLogic {
  public abstract partial record State : StateLogic, IState {
    public record Disabled : State, IGet<Input.Enable> {
      public Disabled() {
        OnEnter<Disabled>(
          previous => Context.Output(new Output.Animations.Idle())
        );

        OnAttach(() => Get<IAppRepo>().GameEntered += OnGameEntered);
        OnDetach(() => Get<IAppRepo>().GameEntered -= OnGameEntered);
      }

      public IState On(Input.Enable input) => new Idle();
    }

    public void OnGameEntered() => Context.Input(new Input.Enable());
  }
}
