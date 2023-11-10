namespace GameDemo.Tests;

using System.Threading.Tasks;
using Chickensoft.AutoInject;
using Chickensoft.GoDotTest;
using Godot;
using GodotTestDriver;
using Moq;
using Shouldly;

public class GameTest : TestClass {
  private Fixture _fixture = default!;
  private Mock<IAppRepo> _appRepo = default!;
  private Mock<IGameRepo> _gameRepo = default!;
  private Mock<IGameLogic> _logic = default!;
  private GameLogic.IFakeBinding _binding = default!;
  private Mock<IPlayerCamera> _playerCam = default!;
  private Mock<IPlayer> _player = default!;
  private Mock<IMap> _map = default!;

  private Game _game = default!;

  public GameTest(Node testScene) : base(testScene) { }

  [Setup]
  public async Task Setup() {
    _fixture = new(TestScene.GetTree());

    _appRepo = new();
    _gameRepo = new();
    _logic = new();
    _binding = GameLogic.CreateFakeBinding();
    _playerCam = new();
    _player = new();
    _map = new();

    _logic.Setup(logic => logic.Bind()).Returns(_binding);

    _game = new() {
      IsTesting = true,
      GameRepo = _gameRepo.Object,
      GameLogic = _logic.Object,
      GameBinding = _binding,
      PlayerCamera = _playerCam.Object,
      Player = _player.Object,
      Map = _map.Object,
    };

    _game.FakeDependency(_appRepo.Object);
    _game.FakeNodeTree(new() {
      ["%PlayerCamera"] = _playerCam.Object,
      ["%Player"] = _player.Object,
      ["%Map"] = _map.Object,
    });

    await _fixture.AddToRoot(_game);
  }

  [Cleanup]
  public async Task Cleanup() {
    await _fixture.Cleanup();
  }

  [Test]
  public void Initializes() {
    _game.Setup();

    _game.GameRepo.ShouldBeOfType<GameRepo>();
    _game.GameBinding.ShouldBe(_binding);
    // Make sure the game provided its dependencies.
    _game.ProviderState.IsInitialized.ShouldBeTrue();
    ((IProvide<IGameRepo>)_game).Value().ShouldNotBeNull();
  }

  [Test]
  public void ChangesToThirdPersonCamera() {
    _logic.Setup(logic => logic.Input(It.IsAny<GameLogic.Input.Initialize>()));
    _game.OnResolved();
    _playerCam.Setup(cam => cam.UsePlayerCamera());

    _binding.Output(new GameLogic.Output.ChangeToThirdPersonCamera());

    _logic.VerifyAll();
    _playerCam.VerifyAll();
  }

  [Test]
  public void SetsPauseMode() {
    _game.OnResolved();
    var tree = TestScene.GetTree();
    tree.Paused.ShouldBeFalse();

    _binding.Output(new GameLogic.Output.SetPauseMode(IsPaused: true));

    tree.Paused.ShouldBeTrue();
    tree.Paused = false;
  }
}
