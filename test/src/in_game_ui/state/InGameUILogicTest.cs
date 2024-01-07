namespace GameDemo.Tests;

using Chickensoft.GoDotTest;
using Godot;
using Moq;
using Shouldly;

public class InGameUILogicTest : TestClass {
  private Mock<IInGameUI> _inGameUi = default!;
  private Mock<IAppRepo> _appRepo = default!;
  private Mock<IGameRepo> _gameRepo = default!;
  private InGameUILogic _logic = default!;

  public InGameUILogicTest(Node testScene) : base(testScene) { }

  [Setup]
  public void Setup() {
    _inGameUi = new Mock<IInGameUI>();
    _appRepo = new Mock<IAppRepo>();
    _gameRepo = new Mock<IGameRepo>();

    _logic =
      new InGameUILogic(_inGameUi.Object, _appRepo.Object, _gameRepo.Object);
  }

  [Test]
  public void Initializes() {
    _logic.Get<IInGameUI>().ShouldBe(_inGameUi.Object);
    _logic.Get<IAppRepo>().ShouldBe(_appRepo.Object);
    _logic.Get<IGameRepo>().ShouldBe(_gameRepo.Object);

    _logic
      .GetInitialState()
      .ShouldBeAssignableTo<InGameUILogic.IState>();
  }
}
