namespace GameDemo.Tests;

using Godot;
using Moq;
using Shouldly;

public class LeavingGameTest(GodotHeadlessFixture godot)
{
  private StateTester _context = default!;
  private Mock<IAppRepo> _appRepo = default!;
  private AppLogicState.LeavingGame _state = default!;
  private AppLogic.Data _data = default!;

  public LeavingGameTest(Node testScene) : base(testScene) { }

  [Setup]
  public void Setup()
  {
    _state = new();
    _appRepo = new();
    _data = new();
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
