namespace GameDemo;

using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;
using Godot;

public partial class JumpshroomLogic {
  public partial record State {
    [Meta]
    public partial record Launching : State, IGet<Input.LaunchCompleted> {
      public IPushEnabled Target { get; set; } = default!;
      public Launching() {
        // We are colliding with something we can push at the moment of
        // launch, so push it.
        this.OnEnter(
          () => Target.Push(Vector3.Up * Get<Data>().ImpulseStrength)
        );
      }

      public Transition On(in Input.LaunchCompleted input) => To<Cooldown>();
    }
  }
}
