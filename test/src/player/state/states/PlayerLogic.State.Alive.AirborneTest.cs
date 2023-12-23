namespace GameDemo.Tests;

using Chickensoft.GoDotTest;
using Godot;
using Shouldly;

public class PlayerLogicStateAliveAirborneTest : TestClass {
  private PlayerLogic.State.Falling _state = default!;

  public PlayerLogicStateAliveAirborneTest(Node testScene) :
    base(testScene) { }

  [Setup]
  public void Setup() {
    _state = new();
  }

  [Test]
  public void HitFloorGoesToMoving() {
    var next = _state.On(new PlayerLogic.Input.HitFloor(true));

    next.ShouldBeOfType<PlayerLogic.State.Moving>();
  }

  [Test]
  public void HitFloorGoesToIdle() {
    var next = _state.On(new PlayerLogic.Input.HitFloor(false));

    next.ShouldBeOfType<PlayerLogic.State.Idle>();
  }

  [Test]
  public void StartedFallingGoesToFalling() {
    var next = _state.On(new PlayerLogic.Input.StartedFalling());

    next.ShouldBeOfType<PlayerLogic.State.Falling>();
  }
}
