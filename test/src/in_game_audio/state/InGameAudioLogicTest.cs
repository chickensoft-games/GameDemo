namespace GameDemo.Tests;
using Chickensoft.GoDotTest;
using Godot;
using Moq;
using Shouldly;

public class InGameAudioLogicTest : TestClass {
  private Mock<IAppRepo> _appRepo = default!;
  private InGameAudioLogic _logic = default!;

  public InGameAudioLogicTest(Node testScene) : base(testScene) { }

  [Setup]
  public void Setup() {
    _appRepo = new Mock<IAppRepo>();

    _logic = new InGameAudioLogic(_appRepo.Object);
  }

  [Test]
  public void Initializes() {
    _logic.Get<IAppRepo>().ShouldBe(_appRepo.Object);
    var context = InGameAudioLogic.CreateFakeContext();
    context.Set(_appRepo.Object);

    _logic
      .GetInitialState(context)
      .ShouldBeAssignableTo<InGameAudioLogic.IState>();
  }
}
