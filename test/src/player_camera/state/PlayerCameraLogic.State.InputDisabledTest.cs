namespace GameDemo.Tests;

using Chickensoft.GoDotTest;
using Godot;
using Moq;
using Shouldly;

public class PlayerCameraLogicStateInputDisabledTest : TestClass
{
  public PlayerCameraLogicStateInputDisabledTest(Node testScene) :
    base(testScene)
  { }

  [Test]
  public void GoesToInputEnabled()
  {
    var state = new PlayerCameraLogic.BaseState.InputDisabled();
    var context = state.Test();
    var gameRepo = new Mock<IGameRepo>();
    var appRepo = new Mock<IAppRepo>();
    context.Set(gameRepo.Object);
    context.Set(appRepo.Object);

    var next = state.On(new PlayerCameraLogic.Input.EnableInput());

    next.IsAssignableTo(typeof(PlayerCameraLogic.BaseState.InputEnabled)).ShouldBeTrue();
  }
}
