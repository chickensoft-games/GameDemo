namespace GameDemo;

using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;

public partial class PlayerLogic
{
  public abstract partial record State
  {
    /// <summary>
    /// This is like the jumping state because it represents moving upwards
    /// through the air with a positive Y velocity, but it doesn't allow the
    /// player to add additional Y velocity since this wasn't triggered by a
    /// jump.
    /// </summary>
    [Meta, Id("player_logic_state_alive_airborne_liftoff")]
    public partial record Liftoff : Airborne
    {
      public Liftoff()
      {
        this.OnEnter(() => Output(new Output.Animations.Jump()));
      }
    }
  }
}
