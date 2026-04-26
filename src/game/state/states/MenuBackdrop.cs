namespace GameDemo;

using System;
using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;

public partial class GameLogic
{
  public partial record BaseState
  {
    [Meta]
    public partial record MenuBackdrop : BaseState,
    IGet<Input.Start>, IGet<Input.Initialize>
    {
      public MenuBackdrop()
      {
        this.OnEnter(() => Get<IGameRepo>().SetIsMouseCaptured(false));
      }

      public Type On(in Input.Start input) => To<Playing>();

      public Type On(in Input.Initialize input)
      {
        Get<IGameRepo>().SetNumCoinsAtStart(input.NumCoinsInWorld);
        return ToSelf();
      }
    }
  }
}
