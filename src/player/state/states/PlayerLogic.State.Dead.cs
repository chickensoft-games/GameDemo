namespace GameDemo;

using Chickensoft.Introspection;

public partial class PlayerLogic
{
  public abstract partial record BaseState
  {
    [Meta, Id("player_logic_state_dead")]
    public partial record Dead : BaseState;
  }
}
