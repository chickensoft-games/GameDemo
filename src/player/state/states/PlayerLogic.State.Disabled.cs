namespace GameDemo;

using Chickensoft.LogicBlocks;

public partial class PlayerLogic {
  public abstract partial record State {
    public record Disabled : State, IGet<Input.Enable> {
      public Disabled() {
        this.OnEnter(() => Output(new Output.Animations.Idle()));

        OnAttach(() => Get<IAppRepo>().GameEntered += OnGameEntered);
        OnDetach(() => Get<IAppRepo>().GameEntered -= OnGameEntered);
      }

      public IState On(in Input.Enable input) => new Idle();
    }

    public void OnGameEntered() => Input(new Input.Enable());
  }
}
