namespace GameDemo;

using System;
using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;

public partial record GameLogicState
{
  [Meta]
  public partial record MenuBackdrop : GameLogicState,
    IGet<Input.Start>, IGet<Input.Initialize>
  {
    public MenuBackdrop()
    {
      this.OnEnter(() => Get<IGameRepo>().SetIsMouseCaptured(false));
    }

    public void OnGameEntered() => Input(new Input.Start());

    public Type On(in Input.Start input) => To<Playing>();

    public Type On(in Input.Initialize input)
    {
      Get<IGameRepo>().SetNumCoinsAtStart(input.NumCoinsInWorld);
      return ToSelf();
    }
  }
}
