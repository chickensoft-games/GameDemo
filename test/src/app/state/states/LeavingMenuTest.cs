namespace GameDemo.Tests;

using Chickensoft.GoDotTest;
using Chickensoft.LogicBlocks;
using Godot;
using Moq;
using Shouldly;

public class LeavingMenuTest : TestClass {
  private IFakeContext _context = default!;
  private Mock<IAppRepo> _appRepo = default!;
  private AppLogic.State.LeavingMenu _state = default!;
  private AppLogic.Data _data = default!;

  public LeavingMenuTest(Node testScene) : base(testScene) { }

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
  public void Enters() {
    _state.Enter();

    _context.Outputs.ShouldBe(
      new object[] { new AppLogic.Output.FadeToBlack() }
    );
  }

  [Test]
  public void StartsGameOnFadeOutFinished() {
    var next = _state.On(new AppLogic.Input.FadeOutFinished());

    next.State.ShouldBeOfType<AppLogic.State.InGame>();
  }

  [Test]
  public void LoadsSaveFileOnFadeOutFinished() {
    _data.ShouldLoadExistingGame = true;

    var next = _state.On(new AppLogic.Input.FadeOutFinished());

    next.State.ShouldBeOfType<AppLogic.State.LoadingSaveFile>();
  }
}
