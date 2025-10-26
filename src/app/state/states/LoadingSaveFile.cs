namespace GameDemo;

using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;

public partial class AppLogic
{
  public partial record State
  {
    [Meta]
    public partial record LoadingSaveFile : State, IGet<Input.SaveFileLoaded>
    {
      public LoadingSaveFile()
      {
        this.OnEnter(() => Output(new Output.StartLoadingSaveFile()));
      }
      public Transition On(in Input.SaveFileLoaded input) => To<InGame>();
    }
  }
}
