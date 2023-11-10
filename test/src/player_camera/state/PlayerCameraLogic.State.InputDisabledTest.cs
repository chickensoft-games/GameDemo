namespace GameDemo.Tests;

using Chickensoft.GoDotTest;
using Godot;
using Moq;
using Shouldly;

public class PlayerCameraLogicStateInputDisabledTest : TestClass {
  public PlayerCameraLogicStateInputDisabledTest(Node testScene) :
    base(testScene) { }

  [Test]
  public void GoesToInputEnabled() {
    var context = PlayerCameraLogic.CreateFakeContext();
    var gameRepo = new Mock<IGameRepo>();
    var appRepo = new Mock<IAppRepo>();
    context.Set(gameRepo.Object);
    context.Set(appRepo.Object);

    var state = new PlayerCameraLogic.State.InputDisabled(context);
    var nextState = state.On(new PlayerCameraLogic.Input.EnableInput());

    nextState.ShouldBeOfType<PlayerCameraLogic.State.InputEnabled>();
  }
}
