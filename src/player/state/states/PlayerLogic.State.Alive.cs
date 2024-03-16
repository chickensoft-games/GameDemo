namespace GameDemo;

using Godot;

public partial class PlayerLogic {
  public partial record State {
    public record Alive : State,
      IGet<Input.PhysicsTick>,
      IGet<Input.Moved>,
      IGet<Input.Pushed>,
      IGet<Input.Killed> {
      // Movement is allowed in any state (even in the air), so these inputs
      // handle movement for each substate unless overridden.
      //
      // If you needed different mechanics, you could move these handlers to
      // a MoveEnabled substate and extend it for states where movement is
      // allowed.

      public virtual IState On(in Input.Killed input) {
        Get<IGameRepo>().OnGameEnded(GameOverReason.Lost);

        return new Dead();
      }

      public virtual IState On(in Input.PhysicsTick input) {
        var delta = input.Delta;
        var player = Get<IPlayer>();
        var settings = Get<Settings>();
        var gameRepo = Get<IGameRepo>();
        var data = Get<Data>();

        var moveDirection =
          player.GetGlobalInputVector(gameRepo.CameraBasis.Value);

        if (moveDirection.Length() > 0.2f) {
          data.LastStrongDirection = moveDirection.Normalized();
        }

        var nextRotationBasis = player.GetNextRotationBasis(
          data.LastStrongDirection, delta, settings.RotationSpeed
        );

        data.LastVelocity = player.Velocity;
        var velocity = data.LastVelocity with { Y = 0f };
        velocity = velocity.Lerp(
          moveDirection * settings.MoveSpeed,
          settings.Acceleration * (float)delta
        );

        if (
          moveDirection.Length() == 0f &&
          velocity.Length() < settings.StoppingSpeed
        ) {
          velocity = Vector3.Zero;
        }

        // Don't clear out Y velocity (in case we're falling/jumping, etc).
        velocity.Y = data.LastVelocity.Y;

        // Add gravity.
        velocity.Y += settings.Gravity * (float)delta;

        Output(
          new Output.MovementComputed(nextRotationBasis, velocity)
        );

        return this;
      }

      public virtual IState On(in Input.Moved input) {
        var player = Get<IPlayer>();
        var settings = Get<Settings>();
        var gameRepo = Get<IGameRepo>();
        var data = Get<Data>();

        // Tell the game the player has moved.
        // Anything that subscribes to our position (like the camera) will
        // be updated.
        gameRepo.SetPlayerGlobalPosition(input.GlobalPosition);

        var speed = (player.Velocity with { Y = 0 }).Length();
        var lastSpeed = (data.LastVelocity with { Y = 0 }).Length();
        var isMovingHorizontally = player.IsMovingHorizontally();
        var isOnFloor = player.IsOnFloor();
        var yVelocity = player.Velocity.Y;
        var hasNegativeYVelocity = player.Velocity.Y < 0f;

        // Determine whether we've *just* encountered a condition that would
        // be relevant to a state change.

        // Leaving the ground is more important than whether or not we've started
        // moving horizontally, since our grounded/airborne states are parent
        // states at the same level of the state hierarchy. Also, horizontal
        // movement is not important for the airborne state, since both jumping
        // and falling allow for horizontal movement, whereas being idle on the
        // ground is only possible while not moving horizontally.
        //
        //         +----------------+               +----------------+
        //   +-----|    Grounded    |----+  +-------|    Airborne    |------+
        //   |     +----------------+    |  |       +----------------+      |
        //   |  +------+     +--------+  |  |  +---------+     +---------+  |
        //   +->| Idle |     | Moving |<-+  +->| Jumping |     | Falling |<-+
        //      +------+     +--------+        +---------+     +---------+

        var justHitFloor = isOnFloor && !data.WasOnFloor;
        var justLeftFloor = !isOnFloor && data.WasOnFloor;
        var justStartedFalling = hasNegativeYVelocity && !data.HadNegativeYVelocity();

        var justStartedMovingHorizontally =
          isMovingHorizontally && !data.WasMovingHorizontally(settings);
        var justStoppedMovingHorizontally =
          !isMovingHorizontally && data.WasMovingHorizontally(settings);

        // Update the cached values so we can use them next frame.
        data.WasOnFloor = isOnFloor;
        data.LastVelocity = player.Velocity;

        if (justHitFloor) {
          Input(
            new Input.HitFloor(IsMovingHorizontally: isMovingHorizontally)
          );
        }
        else if (justLeftFloor) {
          Input(
            new Input.LeftFloor(IsFalling: hasNegativeYVelocity)
          );
        }
        else if (justStartedFalling) {
          Input(new Input.StartedFalling());
        }

        // Grounded status hasn't changed. Check for changes in horizontal
        // movement.

        if (justStartedMovingHorizontally) {
          Input(new Input.StartedMovingHorizontally());
        }
        else if (justStoppedMovingHorizontally) {
          Input(new Input.StoppedMovingHorizontally());
        }

        return this;
      }

      public IState On(in Input.Pushed input) {
        var player = Get<IPlayer>();
        var velocity = player.Velocity;

        // Apply force
        velocity += input.GlobalForceImpulseVector;
        Output(new Output.VelocityChanged(velocity));

        // Remain in current state. Next physics tick will end up applying the
        // force which will make us re-evaluate our state in On(Input.Moved)
        return this;
      }
    }
  }
}
