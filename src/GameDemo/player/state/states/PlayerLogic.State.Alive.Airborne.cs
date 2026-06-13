namespace GameDemo;

using System;
using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;

public partial record PlayerLogicState
{
  [Meta]
  public abstract partial record Airborne : Alive,
    IGet<Input.HitFloor>, IGet<Input.StartedFalling>
  {
    public Type On(in Input.HitFloor input) =>
      input.IsMovingHorizontally ? To<Moving>() : To<Idle>();

    public Type On(in Input.StartedFalling input) => To<Falling>();
  }
}
