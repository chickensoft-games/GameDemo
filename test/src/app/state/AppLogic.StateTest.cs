namespace GameDemo.Tests;

using Chickensoft.GoDotCollections;
using Chickensoft.GoDotTest;
using Godot;
using Moq;
using Shouldly;

public class AppLogicStateTest : TestClass {
  private AppLogic.IFakeContext _context = default!;
  private Mock<IAppRepo> _appRepo = default!;
  private AppLogic.State _state = default!;

  public AppLogicStateTest(Node testScene) : base(testScene) { }

  [Setup]
  public void Setup() {
    _context = AppLogic.CreateFakeContext();
    _appRepo = new();

    _context.Set(_appRepo.Object);
    _state = new(_context);
  }

  [Test]
  public void EntersAndExits() {
    var isMouseCaptured = new Mock<IAutoProp<bool>>();
    _appRepo.Setup(repo => repo.IsMouseCaptured)
      .Returns(isMouseCaptured.Object);

    _state.Enter();

    _appRepo.VerifyAdd(
      repo => repo.IsMouseCaptured.Sync += _state.OnMouseCaptured
    );

    _state.Exit();

    _appRepo.VerifyRemove(
      repo => repo.IsMouseCaptured.Sync -= _state.OnMouseCaptured
    );
  }

  [Test]
  public void RespondsToMouseCapture() {
    _state.OnMouseCaptured(true);
    _state.OnMouseCaptured(false);

    _context.Outputs.ShouldBe(new object[] {
      new AppLogic.Output.CaptureMouse(true),
      new AppLogic.Output.CaptureMouse(false)
    });
  }
}
