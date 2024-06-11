namespace GameDemo.Tests;

using Chickensoft.GoDotCollections;
using Chickensoft.GoDotTest;
using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;
using Godot;
using Moq;
using Shouldly;

public partial class GameLogicStateTest : TestClass {
  [Meta]
  public partial record TestGameState : GameLogic.State;

  private IFakeContext _context = default!;
  private Mock<IGameRepo> _gameRepo = default!;
  private GameLogic.State _state = default!;

  private Mock<IAutoProp<bool>> _isMouseCaptured = default!;
  private Mock<IAutoProp<bool>> _isPaused = default!;

  public GameLogicStateTest(Node testScene) : base(testScene) { }

  [Setup]
  public void Setup() {
    _gameRepo = new();

    _state = new TestGameState();
    _context = _state.CreateFakeContext();
    _context.Set(_gameRepo.Object);

    _isMouseCaptured = new();
    _isPaused = new();

    _gameRepo.Setup(repo => repo.IsMouseCaptured)
      .Returns(_isMouseCaptured.Object);
    _gameRepo.Setup(repo => repo.IsPaused).Returns(_isPaused.Object);
  }

  [Test]
  public void Subscribes() {
    _state.Attach(_context);

    _gameRepo.VerifyAdd(
      repo => repo.IsMouseCaptured.Sync += _state.OnIsMouseCaptured
    );
    _gameRepo.VerifyAdd(
      repo => repo.IsPaused.Sync += _state.OnIsPaused
    );

    _state.Detach();

    _gameRepo.VerifyRemove(
      repo => repo.IsMouseCaptured.Sync -= _state.OnIsMouseCaptured
    );

    _gameRepo.VerifyRemove(
      repo => repo.IsPaused.Sync -= _state.OnIsPaused
    );
  }

  [Test]
  public void OnIsMouseCaptured() {
    _state.OnIsMouseCaptured(true);

    _context.Outputs
      .ShouldBe(new object[] { new GameLogic.Output.CaptureMouse(true) });
  }

  [Test]
  public void OnIsPaused() {
    _state.OnIsPaused(true);

    _context.Outputs
      .ShouldBe(new object[] { new GameLogic.Output.SetPauseMode(true) });
  }
}
