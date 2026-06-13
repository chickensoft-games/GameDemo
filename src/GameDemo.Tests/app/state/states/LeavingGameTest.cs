namespace GameDemo.Tests;

using Moq;
using Shouldly;

public class LeavingGameTest
{
  private readonly StateTester _context;
  private readonly Mock<IAppRepo> _appRepo = new();
  private readonly AppLogicState.LeavingGame _state = new();
  private readonly AppLogic.Data _data = new();

  public LeavingGameTest()
  {
    _context = _state.Test();
    _context.Set(_appRepo.Object);
    _context.Set(_data);
  }

  [Fact]
  public void OnFadeOutFinishedGoesToMainMenu()
  {
    var state = new AppLogicState.LeavingGame();
    var context = state.Test();
    context.Set(new AppLogic.Data()
    {
      PostGameAction = PostGameAction.GoToMainMenu
    });

    var result = state.On(new AppLogicState.Input.FadeOutFinished());

    result.IsAssignableTo(typeof(AppLogicState.MainMenu)).ShouldBeTrue();
    context.Outputs
      .ShouldBe([new AppLogicState.Output.RemoveExistingGame()]);
  }

  [Fact]
  public void OnFadeOutFinishedRestartsGame()
  {
    var state = new AppLogicState.LeavingGame();
    var context = state.Test();
    context.Set(new AppLogic.Data()
    {
      PostGameAction = PostGameAction.RestartGame
    });

    var result = state.On(new AppLogicState.Input.FadeOutFinished());

    result.IsAssignableTo(typeof(AppLogicState.InGame)).ShouldBeTrue();
    context.Outputs.ShouldBe([
      new AppLogicState.Output.RemoveExistingGame(),
      new AppLogicState.Output.SetupGameScene()
    ]);
  }
}
