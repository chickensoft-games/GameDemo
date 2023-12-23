namespace GameDemo.Tests;

using Chickensoft.GoDotTest;
using Godot;
using Moq;
using Shouldly;

public class JumpshroomLogicTest : TestClass {
  private JumpshroomLogic.Data _data = default!;
  private Mock<IAppRepo> _appRepo = default!;
  private JumpshroomLogic _logic = default!;

  public JumpshroomLogicTest(Node testScene) : base(testScene) { }

  [Setup]
  public void Setup() {
    _data = new(1.0f);
    _appRepo = new();

    _logic = new JumpshroomLogic(_data, _appRepo.Object);
  }

  [Test]
  public void Initializes() {
    _logic.Get<JumpshroomLogic.Data>().ShouldBe(_data);
    _logic.Get<IAppRepo>().ShouldBe(_appRepo.Object);

    _logic
      .GetInitialState()
      .ShouldBeAssignableTo<JumpshroomLogic.IState>();
  }
}
