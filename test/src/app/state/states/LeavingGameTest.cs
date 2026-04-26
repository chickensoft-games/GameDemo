namespace GameDemo.Tests;

using Chickensoft.GoDotTest;
using Chickensoft.LogicBlocks;
using Godot;
using Moq;
using Shouldly;

public class LeavingGameTest : TestClass
{
  private StateTester _context = default!;
  private Mock<IAppRepo> _appRepo = default!;
  private AppLogic.BaseState.LeavingGame _state = default!;
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

  [Test]
  public void OnFadeOutFinishedGoesToMainMenu()
  {
    var state = new AppLogic.BaseState.LeavingGame();
    var context = state.Test();
    context.Set(new AppLogic.Data()
    {
      PostGameAction = PostGameAction.GoToMainMenu
    });

    var result = state.On(new AppLogic.Input.FadeOutFinished());

    result.IsAssignableTo(typeof(AppLogic.BaseState.MainMenu)).ShouldBeTrue();
    context.Outputs
      .ShouldBe([new AppLogic.Output.RemoveExistingGame()]);
  }

  [Test]
  public void OnFadeOutFinishedRestartsGame()
  {
    var state = new AppLogic.BaseState.LeavingGame();
    var context = state.Test();
    context.Set(new AppLogic.Data()
    {
      PostGameAction = PostGameAction.RestartGame
    });

    var result = state.On(new AppLogic.Input.FadeOutFinished());

    result.IsAssignableTo(typeof(AppLogic.BaseState.InGame)).ShouldBeTrue();
    context.Outputs.ShouldBe([
      new AppLogic.Output.RemoveExistingGame(),
      new AppLogic.Output.SetupGameScene()
    ]);
  }
}
