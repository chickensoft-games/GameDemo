namespace GameDemo;

public partial class PlayerLogic {
  public abstract partial record State {
    public record Grounded : Alive, IGet<Input.Jump>, IGet<Input.LeftFloor> {
      public virtual IState On(Input.Jump input) {
        // We can jump from any grounded state if the jump button was just
        // pressed.
        var player = Context.Get<IPlayer>();
        var settings = Context.Get<Settings>();

        var velocity = player.Velocity;

        // Start the jump.
        velocity.Y += settings.JumpImpulseForce;
        Context.Output(new Output.VelocityChanged(velocity));

        return new Jumping();
      }

      public IState On(Input.LeftFloor input) {
        if (input.IsFalling) {
          return new Falling();
        }
        // We got pushed into the air by something that isn't the player's jump
        // input, so we have a separate state for that.
        return new Liftoff();
      }
    }
  }
}
