namespace GameDemo;

using System;
using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;
using Godot;

public partial class JumpshroomLogic
{
  public partial record BaseState
  {
    [Meta]
    public partial record Launching : BaseState, IGet<Input.LaunchCompleted>
    {
      public Launching()
      {
        // We are colliding with something we can push at the moment of
        // launch, so push it.
        this.OnEnter(
          () => Get<Data>().Target?.Push(Vector3.Up * Get<Data>().ImpulseStrength)
        );

        this.OnExit(
          () => Get<Data>().Target = null
        );
      }

      public Type On(in Input.LaunchCompleted input) => To<Cooldown>();
    }
  }
}
