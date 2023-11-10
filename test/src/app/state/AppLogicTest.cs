namespace GameDemo.Tests;
using Chickensoft.GoDotTest;
using Godot;
using Moq;
using Shouldly;

public class AppLogicTest : TestClass {
  private Mock<IAppRepo> _appRepo = default!;
  private AppLogic _logic = default!;

  public AppLogicTest(Node testScene) : base(testScene) { }

  [Setup]
  public void Setup() {
    _appRepo = new Mock<IAppRepo>();
    _logic = new AppLogic(_appRepo.Object);
  }

  [Test]
  public void Initializes() {
    _logic.Get<IAppRepo>().ShouldBe(_appRepo.Object);
    var context = AppLogic.CreateFakeContext();
    context.Set(_appRepo.Object);

    _logic
      .GetInitialState(context)
      .ShouldBeAssignableTo<AppLogic.IState>();
  }
}
