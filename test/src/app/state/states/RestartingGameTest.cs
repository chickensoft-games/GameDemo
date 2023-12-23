namespace GameDemo.Tests;

using Chickensoft.GoDotTest;
using Chickensoft.LogicBlocks;
using Godot;
using Moq;
using Shouldly;

public class RestartingGameTest : TestClass {
  private IFakeContext _context = default!;
  private Mock<IAppRepo> _appRepo = default!;
  private AppLogic.State.RestartingGame _state = default!;

  public RestartingGameTest(Node testScene) : base(testScene) { }

  [Setup]
  public void Setup() {
    _appRepo = new();

    _state = new();
    _context = _state.CreateFakeContext();
    _context.Set(_appRepo.Object);
  }

  [Test]
  public void EntersAndExits() {
    var parent = new AppLogic.State();

    _state.Enter(parent);

    _context.Outputs.ShouldBe(
      new object[] { new AppLogic.Output.FadeOut() }
    );

    _context.Reset();

    _state.Exit(parent);

    _context.Outputs.ShouldBe(
      new object[] {
        new AppLogic.Output.RemoveExistingGame(),
        new AppLogic.Output.LoadGame()
      }
    );
  }

  [Test]
  public void RespondsToFadeOutFinished() {
    var next = _state.On(new AppLogic.Input.FadeOutFinished());

    next.ShouldBeOfType<AppLogic.State.PlayingGame>();
  }
}
