namespace GameDemo.Tests;

using Chickensoft.Introspection;
using Chickensoft.LogicBlocks.Auto;
using Godot;
using Shouldly;

public partial class PlayerLogicStateAliveAirborneTest(GodotHeadlessFixture godot)
{
  [Meta, TestState]
  public partial record TestPlayerState : PlayerLogicState.Airborne;

  private PlayerLogicState.Airborne _state = default!;

  public PlayerLogicStateAliveAirborneTest(Node testScene) :
    base(testScene)
  { }

  [Setup]
  public void Setup()
  {
    _state = new TestPlayerState();
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
