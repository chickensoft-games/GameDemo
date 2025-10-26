namespace GameDemo;

using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;

public partial class AppLogic
{
  public partial record State
  {
    [Meta]
    public partial record LeavingMenu : State, IGet<Input.FadeOutFinished>
    {
      public LeavingMenu()
      {
        this.OnEnter(() => Output(new Output.FadeToBlack()));
      }

      public Transition On(in Input.FadeOutFinished input) =>
        Get<Data>().ShouldLoadExistingGame
          ? To<LoadingSaveFile>()
          : To<InGame>();
    }
  }
}
