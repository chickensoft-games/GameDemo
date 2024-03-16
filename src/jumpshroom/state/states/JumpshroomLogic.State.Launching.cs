namespace GameDemo;

using Chickensoft.LogicBlocks;
using Godot;

public partial class JumpshroomLogic : LogicBlock<JumpshroomLogic.IState> {
  public partial record State {
    public record Launching : State, IGet<Input.LaunchCompleted> {
      public IPushEnabled Target { get; }
      public Launching(IPushEnabled target) {
        Target = target;

        // We are colliding with something we can push at the moment of
        // launch, so push it.
        this.OnEnter(
          () => Target.Push(Vector3.Up * Get<Data>().ImpulseStrength)
        );
      }

      public IState On(in Input.LaunchCompleted input) => new Cooldown();
    }
  }
}
