namespace GameDemo.Tests;

using Chickensoft.GoDotTest;
using Godot;
using Moq;
using Shouldly;

public class JumpshroomLogicTest : TestClass {
  private JumpshroomLogic.Data _data = default!;
  private Mock<IGameRepo> _gameRepo = default!;
  private JumpshroomLogic _logic = default!;

  public JumpshroomLogicTest(Node testScene) : base(testScene) { }

  [Setup]
  public void Setup() {
    _data = new(1.0f);
    _gameRepo = new();

    _logic = new JumpshroomLogic(_data, _gameRepo.Object);
  }

  [Test]
  public void Initializes() {
    _logic.Get<JumpshroomLogic.Data>().ShouldBe(_data);
    _logic.Get<IGameRepo>().ShouldBe(_gameRepo.Object);

    _logic
      .GetInitialState()
      .ShouldBeAssignableTo<JumpshroomLogic.IState>();
  }
}
