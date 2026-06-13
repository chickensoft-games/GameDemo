namespace GameDemo;

using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;

public partial record PlayerLogicState
{
  [Meta, Id("player_logic_state_alive_airborne_falling")]
  public partial record Falling : Airborne
  {
    public Falling()
    {
      this.OnEnter(() => Output(new Output.Animations.Fall()));
    }
  }
}
