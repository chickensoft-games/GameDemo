namespace GameDemo.Tests;

using Chickensoft.GoDotCollections;
using Chickensoft.GoDotTest;
using Chickensoft.LogicBlocks;
using Godot;
using Moq;
using Shouldly;

public class AppLogicStateTest : TestClass {
  private IFakeContext _context = default!;
  private Mock<IAppRepo> _appRepo = default!;
  private AppLogic.State _state = default!;

  public AppLogicStateTest(Node testScene) : base(testScene) { }

  [Setup]
  public void Setup() {
    _appRepo = new();

    _state = new();
    _context = _state.CreateFakeContext();
    _context.Set(_appRepo.Object);
  }

  [Test]
  public void Subscribes() {
    var isMouseCaptured = new Mock<IAutoProp<bool>>();
    _appRepo.Setup(repo => repo.IsMouseCaptured)
      .Returns(isMouseCaptured.Object);

    _state.Attach(_context);

    _appRepo.VerifyAdd(
      repo => repo.IsMouseCaptured.Sync += _state.OnMouseCaptured
    );

    _state.Detach();

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
