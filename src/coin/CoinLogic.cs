namespace GameDemo;

using System;
using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;
using Chickensoft.LogicBlocks.Auto;
using Chickensoft.Serialization;

public interface ICoinLogic : IAutoLogicBlock;

[Meta, Id("coin_logic")]
public partial class CoinLogic : AutoBlock, ICoinLogic
{
  public CoinLogic()
  {
    Preallocate<CoinLogicState>();
  }

  public record Settings(double CollectionTimeInSeconds);

  public override ILogicBlockSaveData Serialize(LogicBlockData data) =>
    new CoinLogicSaveData { Data = data };
}

[Meta, Id("coin_logic_save_data")]
public partial class CoinLogicSaveData : ILogicBlockSaveData
{
  [Save("data")]
  public required LogicBlockData Data { get; init; }
}
