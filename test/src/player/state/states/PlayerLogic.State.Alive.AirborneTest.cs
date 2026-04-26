namespace GameDemo.Tests;

using Chickensoft.GoDotTest;
using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;
using Godot;
using Shouldly;

public partial class PlayerLogicStateAliveAirborneTest : TestClass
{
  [Meta]
  public partial record TestPlayerState : PlayerLogic.BaseState.Airborne;

  private PlayerLogic.BaseState.Airborne _state = default!;

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
    var next = _state.On(new PlayerLogic.Input.HitFloor(true));

    next.IsAssignableTo(typeof(PlayerLogic.BaseState.Moving)).ShouldBeTrue();
  }

  [Test]
  public void HitFloorGoesToIdle()
  {
    var next = _state.On(new PlayerLogic.Input.HitFloor(false));

    next.IsAssignableTo(typeof(PlayerLogic.BaseState.Idle)).ShouldBeTrue();
  }

  [Test]
  public void StartedFallingGoesToFalling()
  {
    var next = _state.On(new PlayerLogic.Input.StartedFalling());

    next.IsAssignableTo(typeof(PlayerLogic.BaseState.Falling)).ShouldBeTrue();
  }
}
