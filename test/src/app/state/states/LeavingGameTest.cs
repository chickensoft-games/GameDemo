namespace GameDemo.Tests;

using Chickensoft.GoDotTest;
using Chickensoft.LogicBlocks;
using Godot;
using Moq;
using Shouldly;

public class LeavingGameTest : TestClass {
  private IFakeContext _context = default!;
  private Mock<IAppRepo> _appRepo = default!;
  private AppLogic.State.LeavingGame _state = default!;
  private AppLogic.Data _data = default!;

  public LeavingGameTest(Node testScene) : base(testScene) { }

  [Setup]
  public void Setup() {
    _state = new();
    _appRepo = new();
    _data = new();
    _context = _state.CreateFakeContext();
    _context.Set(_appRepo.Object);
    _context.Set(_data);
  }

  [Test]
  public void OnFadeOutFinishedGoesToMainMenu() {
    var state = new AppLogic.State.LeavingGame() {
      PostGameAction = PostGameAction.GoToMainMenu
    };
    var context = state.CreateFakeContext();

    var result = state.On(new AppLogic.Input.FadeOutFinished());

    result.State.ShouldBeOfType<AppLogic.State.MainMenu>();
    context.Outputs
      .ShouldBe([new AppLogic.Output.RemoveExistingGame()]);
  }

  [Test]
  public void OnFadeOutFinishedRestartsGame() {
    var state = new AppLogic.State.LeavingGame() {
      PostGameAction = PostGameAction.RestartGame
    };
    var context = state.CreateFakeContext();

    var result = state.On(new AppLogic.Input.FadeOutFinished());

    result.State.ShouldBeOfType<AppLogic.State.InGame>();
    context.Outputs.ShouldBe([
      new AppLogic.Output.RemoveExistingGame(),
      new AppLogic.Output.SetupGameScene()
    ]);
  }
}
