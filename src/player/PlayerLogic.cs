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
  public PlayerLogic()
  {
    Preallocate<PlayerLogicState>();
  }

  public override IEnumerable<IDisposable> OnStartSubscriptions()
  {
    yield return Get<IAppRepo>().AutoChannel.Bind()
      .On((in IAppRepo.GameEntering _) => (State as PlayerLogicState.Disabled)?.OnGameEntered());
  }

  public override ILogicBlockSaveData Serialize(LogicBlockData data) =>
    new PlayerLogicSaveData { Data = data };
}

[Meta, Id("player_logic_save_data")]
public partial class PlayerLogicSaveData : ILogicBlockSaveData
{
  [Save("data")]
  public required LogicBlockData Data { get; init; }
}
