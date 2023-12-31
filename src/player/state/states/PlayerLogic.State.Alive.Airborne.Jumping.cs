namespace GameDemo;

public partial class PlayerLogic {
  public abstract partial record State {
    public record Jumping : Airborne, IGet<Input.Jump> {
      public Jumping() {
        OnEnter<Jumping>(
          previous => {
            Context.Output(new Output.Animations.Jump());
            Get<IGameRepo>().Jump();
          }
        );
      }

      // Override jump when in the air to allow for bigger jumps if the player
      // keeps holding down the jump button.
      public IState On(Input.Jump input) {
        var player = Context.Get<IPlayer>();
        var settings = Context.Get<Settings>();

        var velocity = player.Velocity;

        // Continue the jump in-air. Very forgiving player physics.
        velocity.Y += settings.JumpForce * (float)input.Delta;
        Context.Output(new Output.VelocityChanged(velocity));

        return this;
      }
    }
  }
}
