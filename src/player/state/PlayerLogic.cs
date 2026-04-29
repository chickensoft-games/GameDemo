namespace GameDemo;

using System;
using System.Collections.Generic;
using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;
using Chickensoft.LogicBlocks.Auto;
using Chickensoft.Serialization;

public interface IPlayerLogic : IAutoLogicBlock;

[Meta, Id("player_logic")]
public partial class PlayerLogic : AutoBlock, IPlayerLogic
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
    yield return SetupSubscriptions(Get<IAppRepo>(), () => State);
  }

  public static IDisposable SetupSubscriptions(IAppRepo appRepo, Func<LogicBlockState?> stateFunc)
  {
    return appRepo.AutoChannel.Bind().On((
      in IAppRepo.GameEntered _) => stateFunc()?.Input(new Input.Enable()));
  }

  public override ILogicBlockSaveData GetSaveData(LogicBlockData data) =>
    new PlayerLogicSaveData { Data = data };
}

[Meta, Id("player_logic_save_data")]
public partial class PlayerLogicSaveData : ILogicBlockSaveData
{
  [Save("data")]
  public required LogicBlockData Data { get; init; }
}
