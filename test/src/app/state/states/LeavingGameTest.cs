namespace GameDemo.Tests;

using Chickensoft.GoDotTest;
using Godot;
using Shouldly;

public class LeavingGameTest : TestClass {
  public LeavingGameTest(Node testScene) : base(testScene) { }

  [Test]
  public void OnFadeOutFinishedGoesToMainMenu() {
    var state = new AppLogic.State.LeavingGame(PostGameAction.GoToMainMenu);
    var context = state.CreateFakeContext();

    var result = state.On(new AppLogic.Input.FadeOutFinished());

    result.ShouldBeOfType<AppLogic.State.MainMenu>();
    context.Outputs
      .ShouldBe(new object[] { new AppLogic.Output.RemoveExistingGame() });
  }

  [Test]
  public void OnFadeOutFinishedRestartsGame() {
    var state = new AppLogic.State.LeavingGame(PostGameAction.RestartGame);
    var context = state.CreateFakeContext();

    var result = state.On(new AppLogic.Input.FadeOutFinished());

    result.ShouldBeOfType<AppLogic.State.InGame>();
    context.Outputs.ShouldBe(new object[] {
      new AppLogic.Output.RemoveExistingGame(), new AppLogic.Output.LoadGame()
    });
  }
}
