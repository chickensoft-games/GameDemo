namespace GameDemo;

using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;

public partial class JumpshroomLogic
{
  public partial record State
  {
    [Meta]
    public partial record Loading : State, IGet<Input.Launch>
    {
      public IPushEnabled Target { get; set; } = default!;

      public Loading()
      {
        this.OnEnter(() =>
        {
          Get<IGameRepo>().OnJumpshroomUsed();
          Output(new Output.Animate());
        });
      }

      // Springy top is fully compressed, so it is ready to launch.
      public Transition On(in Input.Launch input) => To<Launching>()
        .With(state => ((Launching)state).Target = Target);
    }
  }
}
