namespace GameDemo;

using System;
using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;

public interface ICoinLogic : ILogicBlock;

[Meta, Id("coin_logic")]
public partial class CoinLogic : LogicBlock, ICoinLogic
{
  public override Type GetInitialState() => typeof(State.Idle);

  public CoinLogic()
  {
    Set(new State.Collecting());
    Set(new State.Idle());
  }

  public record Settings(double CollectionTimeInSeconds);
}
