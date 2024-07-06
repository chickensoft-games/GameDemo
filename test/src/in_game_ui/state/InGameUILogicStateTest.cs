namespace GameDemo.Tests;

using Chickensoft.Collections;
using Chickensoft.GoDotTest;
using Chickensoft.LogicBlocks;
using Godot;
using Moq;
using Shouldly;

public class InGameUILogicStateTest : TestClass {
  private IFakeContext _context = default!;
  private Mock<IInGameUI> _inGameUi = default!;
  private Mock<IAppRepo> _appRepo = default!;
  private Mock<IGameRepo> _gameRepo = default!;
  private InGameUILogic.State _state = default!;

  public InGameUILogicStateTest(Node testScene) : base(testScene) { }

  [Setup]
  public void Setup() {
    _inGameUi = new Mock<IInGameUI>();
    _appRepo = new Mock<IAppRepo>();
    _gameRepo = new Mock<IGameRepo>();

    _state = new InGameUILogic.State();
    _context = _state.CreateFakeContext();

    _context.Set(_inGameUi.Object);
    _context.Set(_appRepo.Object);
    _context.Set(_gameRepo.Object);
  }

  [Test]
  public void Subscribes() {
    var numCoinsCollected = new Mock<IAutoProp<int>>();
    var numCoinsAtStart = new Mock<IAutoProp<int>>();

    _gameRepo.Setup(repo => repo.NumCoinsCollected)
      .Returns(numCoinsCollected.Object);
    _gameRepo.Setup(repo => repo.NumCoinsAtStart)
      .Returns(numCoinsAtStart.Object);

    _state.Attach(_context);

    _gameRepo
      .VerifyAdd(x => x.NumCoinsCollected.Sync += _state.OnNumCoinsCollected);
    _gameRepo
      .VerifyAdd(x => x.NumCoinsAtStart.Sync += _state.OnNumCoinsAtStart);

    _state.Detach();

    _gameRepo
      .VerifyRemove(x =>
        x.NumCoinsCollected.Sync -= _state.OnNumCoinsCollected);
    _gameRepo
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
