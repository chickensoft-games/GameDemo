namespace GameDemo;

using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;

public partial class PlayerLogic
{
  public abstract partial record State
  {
    [Meta, Id("player_logic_state_disabled")]
    public partial record Disabled : State, IGet<Input.Enable>
    {
      public Disabled()
      {
        this.OnEnter(() => Output(new Output.Animations.Idle()));

        OnAttach(() => Get<IAppRepo>().GameEntered += OnGameEntered);
        OnDetach(() => Get<IAppRepo>().GameEntered -= OnGameEntered);
      }

      public Transition On(in Input.Enable input) => To<Idle>();
    }

    public void OnGameEntered() => Input(new Input.Enable());
  }
}
