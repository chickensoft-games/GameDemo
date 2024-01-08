namespace GameDemo.Tests;

using Chickensoft.GoDotTest;
using Godot;
using Moq;
using Shouldly;

public class InGameAudioLogicTest : TestClass {
  private Mock<IAppRepo> _appRepo = default!;
  private Mock<IGameRepo> _gameRepo = default!;
  private InGameAudioLogic _logic = default!;

  public InGameAudioLogicTest(Node testScene) : base(testScene) { }

  [Setup]
  public void Setup() {
    _appRepo = new Mock<IAppRepo>();
    _gameRepo = new Mock<IGameRepo>();

    _logic = new InGameAudioLogic(_appRepo.Object, _gameRepo.Object);
  }

  [Test]
  public void Initializes() {
    _logic.Get<IAppRepo>().ShouldBe(_appRepo.Object);
    _logic.Get<IGameRepo>().ShouldBe(_gameRepo.Object);

    _logic
      .GetInitialState()
      .ShouldBeAssignableTo<InGameAudioLogic.IState>();
  }
}
