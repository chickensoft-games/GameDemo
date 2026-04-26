namespace GameDemo;

using System;
using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;
using Chickensoft.LogicBlocks.Auto;

public interface ICoinLogic : IAutoLogicBlock;

[Meta, Id("coin_logic")]
public partial class CoinLogic : AutoBlock, ICoinLogic
{
  public override Type GetInitialState() => typeof(State.Idle);

  public CoinLogic()
  {
    Set(new State.Collecting());
    Set(new State.Idle());
  }

  public record Settings(double CollectionTimeInSeconds);

  public override ILogicBlockSaveData GetSaveData(LogicBlockData data) =>
    new CoinLogicSaveData { Data = data };
}

[Meta, Id("coin_logic_save_data")]
public partial class CoinLogicSaveData : ILogicBlockSaveData
{
  public required LogicBlockData Data { get; init; }
}
