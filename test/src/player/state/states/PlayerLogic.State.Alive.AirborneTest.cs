namespace GameDemo.Tests;

using Chickensoft.GoDotTest;
using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;
using Godot;
using Shouldly;

public partial class PlayerLogicStateAliveAirborneTest : TestClass
{
  [Meta, TestState]
  public partial record TestPlayerState : PlayerLogic.State.Airborne;

  private PlayerLogic.State.Airborne _state = default!;

  public PlayerLogicStateAliveAirborneTest(Node testScene) :
    base(testScene)
  { }

  [Setup]
  public void Setup()
  {
    _state = new TestPlayerState();
    _state.CreateFakeContext();
  }

  [Test]
  public void HitFloorGoesToMoving()
  {
    var next = _state.On(new PlayerLogic.Input.HitFloor(true));

    next.State.ShouldBeOfType<PlayerLogic.State.Moving>();
  }

  [Test]
  public void HitFloorGoesToIdle()
  {
    var next = _state.On(new PlayerLogic.Input.HitFloor(false));

    next.State.ShouldBeOfType<PlayerLogic.State.Idle>();
  }

  [Test]
  public void StartedFallingGoesToFalling()
  {
    var next = _state.On(new PlayerLogic.Input.StartedFalling());

    next.State.ShouldBeOfType<PlayerLogic.State.Falling>();
  }
}
