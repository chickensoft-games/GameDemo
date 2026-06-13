namespace GameDemo.Tests;

using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;
using Chickensoft.LogicBlocks.Auto;
using Godot;
using Shouldly;

public partial class PlayerLogicStateAliveAirborneTest : TestClass
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

  [Test]
  public void HitFloorGoesToMoving()
  {
    var next = _state.On(new PlayerLogicState.Input.HitFloor(true));

    next.IsAssignableTo(typeof(PlayerLogicState.Moving)).ShouldBeTrue();
  }

  [Test]
  public void HitFloorGoesToIdle()
  {
    var next = _state.On(new PlayerLogicState.Input.HitFloor(false));

    next.IsAssignableTo(typeof(PlayerLogicState.Idle)).ShouldBeTrue();
  }

  [Test]
  public void StartedFallingGoesToFalling()
  {
    var next = _state.On(new PlayerLogicState.Input.StartedFalling());

    next.IsAssignableTo(typeof(PlayerLogicState.Falling)).ShouldBeTrue();
  }
}
