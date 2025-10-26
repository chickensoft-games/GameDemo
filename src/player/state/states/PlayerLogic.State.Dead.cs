namespace GameDemo;

using Chickensoft.Introspection;

public partial class PlayerLogic
{
  public abstract partial record State
  {
    [Meta, Id("player_logic_state_dead")]
    public partial record Dead : State;
  }
}
