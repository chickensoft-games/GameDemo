namespace GameDemo;

using System;
using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;

public partial record AppLogicState
{
  [Meta]
  public partial record LoadingSaveFile : AppLogicState, IGet<Input.SaveFileLoaded>
  {
    public LoadingSaveFile()
    {
      this.OnEnter(() => Output(new Output.StartLoadingSaveFile()));
    }
    public Type On(in Input.SaveFileLoaded input) => To<InGame>();
  }
}
