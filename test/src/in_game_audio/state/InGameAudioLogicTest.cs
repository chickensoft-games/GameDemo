namespace GameDemo.Tests;

using Chickensoft.GoDotTest;
using Godot;
using Shouldly;

public class InGameAudioLogicTest : TestClass {
  private InGameAudioLogic _logic = default!;

  public InGameAudioLogicTest(Node testScene) : base(testScene) { }

  [Setup]
  public void Setup() {
    _logic = new InGameAudioLogic();
  }

  [Test]
  public void Initializes() {
    _logic
      .GetInitialState().State
      .ShouldBeAssignableTo<InGameAudioLogic.State>();
  }
}
