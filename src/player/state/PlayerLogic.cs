namespace GameDemo;

using System;
using System.Collections.Generic;
using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;

public interface IPlayerLogic : ILogicBlock;

[Meta, Id("player_logic")]
public partial class PlayerLogic : LogicBlock, IPlayerLogic
{
  public override Type GetInitialState() => typeof(BaseState.Disabled);

  public PlayerLogic()
  {
    Set(new BaseState.Falling());
    Set(new BaseState.Jumping());
    Set(new BaseState.Liftoff());
    Set(new BaseState.Idle());
    Set(new BaseState.Moving());
    Set(new BaseState.Dead());
    Set(new BaseState.Disabled());
  }

  public override IEnumerable<IDisposable> OnStartSubscriptions()
  {
    yield return Get<IAppRepo>().AutoChannel.Bind().On((
      in IAppRepo.GameEntered _) => State?.Input(new Input.Enable()));
  }
}
