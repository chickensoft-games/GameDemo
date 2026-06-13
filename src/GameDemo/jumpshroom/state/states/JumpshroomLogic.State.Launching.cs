namespace GameDemo;

using System;
using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;
using Godot;

public partial record JumpshroomLogicState
{
  [Meta]
  public partial record Launching : JumpshroomLogicState, IGet<Input.LaunchCompleted>
  {
    public Launching()
    {
      // We are colliding with something we can push at the moment of
      // launch, so push it.
      this.OnEnter(
        () => Get<JumpshroomLogic.Data>().Target?.Push(Vector3.Up * Get<JumpshroomLogic.Data>().ImpulseStrength)
      );

      this.OnExit(
        () => Get<JumpshroomLogic.Data>().Target = null
      );
    }

    public Type On(in Input.LaunchCompleted input) => To<Cooldown>();
  }
}
