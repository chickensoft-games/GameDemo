namespace GameDemo.Tests;

using Chickensoft.GoDotTest;
using Godot;
using Moq;
using Shouldly;

public class GameLogicTest : TestClass {
  private Mock<IGameRepo> _gameRepo = default!;
  private Mock<IAppRepo> _appRepo = default!;
  private GameLogic _logic = default!;

  public GameLogicTest(Node testScene) : base(testScene) { }

  [Setup]
  public void Setup() {
    _gameRepo = new();
    _appRepo = new();

    _logic = new GameLogic(_gameRepo.Object, _appRepo.Object);
  }

  [Test]
  public void Initializes() {
    _logic.Get<IAppRepo>().ShouldBe(_appRepo.Object);
    _logic.Get<IGameRepo>().ShouldBe(_gameRepo.Object);

    _logic
      .GetInitialState()
      .ShouldBeAssignableTo<GameLogic.IState>();
  }
}
