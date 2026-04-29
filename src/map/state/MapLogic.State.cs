namespace GameDemo;

using System;
using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;
public partial class MapLogic
{
  [Meta]
  public partial record BaseState : LogicBlockState,
  IGet<Input.GameLoadedFromSaveFile>
  {
    public Type On(in Input.GameLoadedFromSaveFile input)
    {
      Get<IGameRepo>().SetNumCoinsCollected(input.NumCoinsCollected);
      return ToSelf();
    }
  }
}
