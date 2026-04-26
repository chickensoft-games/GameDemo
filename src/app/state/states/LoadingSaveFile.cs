namespace GameDemo;

using System;
using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;

public partial class AppLogic
{
  public partial record BaseState
  {
    [Meta]
    public partial record LoadingSaveFile : BaseState, IGet<Input.SaveFileLoaded>
    {
      public LoadingSaveFile()
      {
        this.OnEnter(() => Output(new Output.StartLoadingSaveFile()));
      }
      public Type On(in Input.SaveFileLoaded input) => To<InGame>();
    }
  }
}
