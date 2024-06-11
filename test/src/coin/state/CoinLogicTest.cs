namespace GameDemo.Tests;

using Chickensoft.GoDotTest;
using Godot;
using Shouldly;

public class CoinLogicTest : TestClass {
  private CoinLogic _logic = default!;

  public CoinLogicTest(Node testScene) : base(testScene) { }

  [Setup]
  public void Setup() {
    _logic = new CoinLogic();
  }

  [Test]
  public void Initializes() {
    _logic
      .GetInitialState().State
      .ShouldBeAssignableTo<CoinLogic.State.Idle>();
  }
}
