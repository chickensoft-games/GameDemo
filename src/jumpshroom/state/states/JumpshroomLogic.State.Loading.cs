namespace GameDemo;

using System;
using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;

public partial record JumpshroomLogicState
{
  [Meta]
  public partial record Loading : JumpshroomLogicState, IGet<Input.Launch>
  {
    public Loading()
    {
      this.OnEnter(() =>
      {
        Get<IGameRepo>().OnJumpshroomUsed();
        Output(new Output.Animate());
      });
    }

    // Springy top is fully compressed, so it is ready to launch.
    public Type On(in Input.Launch input) => To<Launching>();
  }
}
