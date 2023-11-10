namespace GameDemo.Tests;

using Chickensoft.GoDotTest;
using Godot;
using Moq;
using Shouldly;

public class LeavingGameTest : TestClass {
  private AppLogic.IFakeContext _context = default!;
  private Mock<IAppRepo> _appRepo = default!;
  private AppLogic.State.LeavingGame _state = default!;

  public LeavingGameTest(Node testScene) : base(testScene) { }

  [Setup]
  public void Setup() {
    _context = AppLogic.CreateFakeContext();
    _appRepo = new();

    _context.Set(_appRepo.Object);
    _state = new(_context);
  }

  [Test]
  public void EntersAndExits() {
    var parent = new AppLogic.State.InGame(_context);

    _state.Enter(parent);
    _context.Outputs.ShouldBe(
      new object[] { new AppLogic.Output.FadeOut() }
    );

    _context.Reset();

    _state.Exit(parent);
    _context.Outputs.ShouldBe(
      new object[] { new AppLogic.Output.RemoveExistingGame() }
    );
  }

  [Test]
  public void TellsAppGameEndedOnFadeOut() {
    _appRepo.Setup((repo) => repo.OnGameEnded(GameOverReason.Exited));

    var next = _state.On(new AppLogic.Input.FadeOutFinished());

    _appRepo.VerifyAll();

    next.ShouldBeOfType<AppLogic.State.LeavingGame>();
  }
}
