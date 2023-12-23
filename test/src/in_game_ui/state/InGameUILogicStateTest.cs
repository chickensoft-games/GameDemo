namespace GameDemo.Tests;

using Chickensoft.GoDotCollections;
using Chickensoft.GoDotTest;
using Chickensoft.LogicBlocks;
using Godot;
using Moq;
using Shouldly;

public class InGameUILogicStateTest : TestClass {
  private IFakeContext _context = default!;
  private Mock<IInGameUI> _inGameUi = default!;
  private Mock<IAppRepo> _appRepo = default!;
  private InGameUILogic.State _state = default!;

  public InGameUILogicStateTest(Node testScene) : base(testScene) { }

  [Setup]
  public void Setup() {
    _inGameUi = new Mock<IInGameUI>();
    _appRepo = new Mock<IAppRepo>();

    _state = new InGameUILogic.State();
    _context = _state.CreateFakeContext();

    _context.Set(_inGameUi.Object);
    _context.Set(_appRepo.Object);
  }

  [Test]
  public void EntersAndExits() {
    var numCoinsCollected = new Mock<IAutoProp<int>>();
    var numCoinsAtStart = new Mock<IAutoProp<int>>();

    _appRepo.Setup(repo => repo.NumCoinsCollected)
      .Returns(numCoinsCollected.Object);
    _appRepo.Setup(repo => repo.NumCoinsAtStart)
      .Returns(numCoinsAtStart.Object);

    _state.Attach(_context);

    _appRepo
      .VerifyAdd(x => x.NumCoinsCollected.Sync += _state.OnNumCoinsCollected);
    _appRepo
      .VerifyAdd(x => x.NumCoinsAtStart.Sync += _state.OnNumCoinsAtStart);

    _state.Detach();

    _appRepo
      .VerifyRemove(x => x.NumCoinsCollected.Sync -= _state.OnNumCoinsCollected);
    _appRepo
      .VerifyRemove(x => x.NumCoinsAtStart.Sync -= _state.OnNumCoinsAtStart);
  }

  [Test]
  public void OnNumCoinsCollectedOutputs() {
    _state.OnNumCoinsCollected(5);

    _context.Outputs.ShouldBe(new object[] {
      new InGameUILogic.Output.NumCoinsCollectedChanged(5)
    });
  }

  [Test]
  public void OnNumCoinsAtStartOutputs() {
    _state.OnNumCoinsAtStart(10);

    _context.Outputs.ShouldBe(new object[] {
      new InGameUILogic.Output.NumCoinsAtStartChanged(10)
    });
  }
}
