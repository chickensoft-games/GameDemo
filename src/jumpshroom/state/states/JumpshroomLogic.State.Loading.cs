namespace GameDemo;

using Chickensoft.LogicBlocks;

public partial class JumpshroomLogic : LogicBlock<JumpshroomLogic.IState> {
  public partial record State {
    public record Loading : State, IGet<Input.Launch> {
      public IPushEnabled Target { get; }
      public Loading(IContext context, IPushEnabled target) : base(context) {
        Target = target;

        var appRepo = context.Get<IAppRepo>();

        OnEnter<Loading>((previous) => {
          appRepo.OnJumpshroomUsed();
          Context.Output(new Output.Animate());
        });
      }

      public IState On(Input.Launch input) {
        // Springy top is fully compressed, so it is ready to launch.
        return new Launching(Context, Target);
      }
    }
  }
}
