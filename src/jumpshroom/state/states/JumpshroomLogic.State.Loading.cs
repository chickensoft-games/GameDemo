namespace GameDemo;

using Chickensoft.LogicBlocks;

public partial class JumpshroomLogic : LogicBlock<JumpshroomLogic.IState> {
  public partial record State {
    public record Loading : State, IGet<Input.Launch> {
      public IPushEnabled Target { get; }
      public Loading(IPushEnabled target) {
        Target = target;

        OnEnter<Loading>((previous) => {
          Get<IAppRepo>().OnJumpshroomUsed();
          Context.Output(new Output.Animate());
        });
      }

      public IState On(Input.Launch input) {
        // Springy top is fully compressed, so it is ready to launch.
        return new Launching(Target);
      }
    }
  }
}
