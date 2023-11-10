namespace GameDemo.Tests;
using Chickensoft.GoDotTest;
using Godot;
using Moq;
using Shouldly;

public class PlayerLogicTest : TestClass {
  private Mock<IPlayer> _player = default!;
  private PlayerLogic.Settings _settings = default!;
  private Mock<IAppRepo> _appRepo = default!;
  private Mock<IGameRepo> _gameRepo = default!;
  private PlayerLogic _logic = default!;

  public PlayerLogicTest(Node testScene) : base(testScene) { }

  [Setup]
  public void Setup() {
    _player = new Mock<IPlayer>();
    _settings = new PlayerLogic.Settings(1, 1, 1, 1, 1, 1, 1);
    _appRepo = new Mock<IAppRepo>();
    _gameRepo = new Mock<IGameRepo>();

    _logic = new PlayerLogic(_player.Object, _settings, _appRepo.Object, _gameRepo.Object);
  }

  [Test]
  public void Initializes() {
    _logic.Get<IPlayer>().ShouldBe(_player.Object);
    _logic.Get<PlayerLogic.Settings>().ShouldBe(_settings);
    _logic.Get<IAppRepo>().ShouldBe(_appRepo.Object);
    _logic.Get<IGameRepo>().ShouldBe(_gameRepo.Object);
    _logic.Get<PlayerLogic.Data>().ShouldNotBeNull();

    var context = PlayerLogic.CreateFakeContext();

    context.Set(_appRepo.Object);

    _logic
      .GetInitialState(context)
      .ShouldBeAssignableTo<PlayerLogic.IState>();
  }
}
