namespace GameDemo;

using Chickensoft.Introspection;

public partial class PlayerLogic
{
  public abstract partial record State
  {
    [Meta]
    public abstract partial record Grounded : Alive,
    IGet<Input.Jump>, IGet<Input.LeftFloor>
    {
      public virtual Transition On(in Input.Jump input)
      {
        // We can jump from any grounded state if the jump button was just
        // pressed.
        var player = Get<IPlayer>();
        var settings = Get<Settings>();

        var velocity = player.Velocity;

        // Start the jump.
        velocity.Y += settings.JumpImpulseForce;
        Output(new Output.VelocityChanged(velocity));

        return To<Jumping>();
      }

      public Transition On(in Input.LeftFloor input)
      {
        if (input.IsFalling)
        {
          return To<Falling>();
        }
        // We got pushed into the air by something that isn't the player's jump
        // input, so we have a separate state for that.
        return To<Liftoff>();
      }
    }
  }
}
