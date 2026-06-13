namespace GameDemo.Tests;

using Chickensoft.Introspection;
using Chickensoft.LogicBlocks.Auto;
using Shouldly;

public partial class PlayerLogicStateAliveAirborneTest
{
  [Meta, TestState]
  public partial record TestPlayerState : PlayerLogicState.Airborne;

  private readonly PlayerLogicState.Airborne _state = new TestPlayerState();

  public PlayerLogicStateAliveAirborneTest()
  {
    _state.Test();
  }

  [Fact]
  public void HitFloorGoesToMoving()
  {
    var next = _state.On(new PlayerLogicState.Input.HitFloor(true));

    next.IsAssignableTo(typeof(PlayerLogicState.Moving)).ShouldBeTrue();
  }

  [Fact]
  public void HitFloorGoesToIdle()
  {
    var next = _state.On(new PlayerLogicState.Input.HitFloor(false));

    next.IsAssignableTo(typeof(PlayerLogicState.Idle)).ShouldBeTrue();
  }

  [Fact]
  public void StartedFallingGoesToFalling()
  {
    var next = _state.On(new PlayerLogicState.Input.StartedFalling());

    next.IsAssignableTo(typeof(PlayerLogicState.Falling)).ShouldBeTrue();
  }
}
