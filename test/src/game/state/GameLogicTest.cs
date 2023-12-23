namespace GameDemo.Tests;
using Chickensoft.GoDotTest;
using Godot;
using Moq;
using Shouldly;

public class GameLogicTest : TestClass {
  private Mock<IAppRepo> _appRepo = default!;
  private GameLogic _logic = default!;

  public GameLogicTest(Node testScene) : base(testScene) { }

  [Setup]
  public void Setup() {
    _appRepo = new Mock<IAppRepo>();

    _logic = new GameLogic(_appRepo.Object);
  }

  [Test]
  public void Initializes() {
    _logic.Get<IAppRepo>().ShouldBe(_appRepo.Object);

    _logic
      .GetInitialState()
      .ShouldBeAssignableTo<GameLogic.IState>();
  }
}
