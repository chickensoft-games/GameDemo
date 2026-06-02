namespace GameDemo;

using Chickensoft.Introspection;

public abstract partial record PlayerLogicState
{
  [Meta, Id("player_logic_state_dead")]
  public partial record Dead : PlayerLogicState;
}
