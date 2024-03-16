namespace GameDemo;

using Chickensoft.LogicBlocks;

public partial class PlayerLogic {
  public abstract partial record State {
    public record Jumping : Airborne, IGet<Input.Jump> {
      public Jumping() {
        this.OnEnter(
          () => {
            Output(new Output.Animations.Jump());
            Get<IGameRepo>().OnJump();
          }
        );
      }

      // Override jump when in the air to allow for bigger jumps if the player
      // keeps holding down the jump button.
      public IState On(in Input.Jump input) {
        var player = Get<IPlayer>();
        var settings = Get<Settings>();

        var velocity = player.Velocity;

        // Continue the jump in-air. Very forgiving player physics.
        velocity.Y += settings.JumpForce * (float)input.Delta;
        Output(new Output.VelocityChanged(velocity));

        return this;
      }
    }
  }
}
