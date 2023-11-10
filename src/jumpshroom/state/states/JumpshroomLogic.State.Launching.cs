namespace GameDemo;

using Chickensoft.LogicBlocks;
using Godot;

public partial class JumpshroomLogic : LogicBlock<JumpshroomLogic.IState> {
  public partial record State {
    public record Launching : State, IGet<Input.LaunchCompleted> {
      public IPushEnabled Target { get; }
      public Launching(IContext context, IPushEnabled target) : base(context) {
        Target = target;
        OnEnter<Launching>((previous) => {
          var data = Context.Get<Data>();
          // We are colliding with something we can push at the moment of
          // launch, so push it.
          Target.Push(Vector3.Up * data.ImpulseStrength);
        });
      }

      public IState On(Input.LaunchCompleted input) => new Cooldown(Context);
    }
  }
}
