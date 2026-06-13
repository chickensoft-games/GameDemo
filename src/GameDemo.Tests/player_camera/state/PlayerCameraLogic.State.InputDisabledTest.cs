namespace GameDemo.Tests;

using Godot;
using Moq;
using Shouldly;

public class PlayerCameraLogicStateInputDisabledTest(GodotHeadlessFixture godot)
{
  public PlayerCameraLogicStateInputDisabledTest(Node testScene) :
    base(testScene)
  { }

  [Fact]
  public void GoesToInputEnabled()
  {
    var state = new PlayerCameraLogicState.InputDisabled();
    var context = state.Test();
    var gameRepo = new Mock<IGameRepo>();
    var appRepo = new Mock<IAppRepo>();
    context.Set(gameRepo.Object);
    context.Set(appRepo.Object);

    var next = state.On(new PlayerCameraLogicState.Input.EnableInput());

    next.IsAssignableTo(typeof(PlayerCameraLogicState.InputEnabled)).ShouldBeTrue();
  }
}
