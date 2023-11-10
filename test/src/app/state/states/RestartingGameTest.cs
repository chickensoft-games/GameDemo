namespace GameDemo.Tests;

using Chickensoft.GoDotTest;
using Godot;
using Moq;
using Shouldly;

public class RestartingGameTest : TestClass {
  private AppLogic.IFakeContext _context = default!;
  private Mock<IAppRepo> _appRepo = default!;
  private AppLogic.State.RestartingGame _state = default!;

  public RestartingGameTest(Node testScene) : base(testScene) { }

  [Setup]
  public void Setup() {
    _context = AppLogic.CreateFakeContext();
    _appRepo = new();

    _context.Set(_appRepo.Object);
    _state = new(_context);
  }

  [Test]
  public void EntersAndExits() {
    var parent = new AppLogic.State(_context);

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
