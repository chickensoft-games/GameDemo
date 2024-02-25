namespace GameDemo.Tests;

using System.Threading.Tasks;
using Chickensoft.AutoInject;
using Chickensoft.GoDotTest;
using Godot;
using Chickensoft.GodotTestDriver;
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
  private Mock<IInGameUI> _ui = default!;
  private Mock<IDeathMenu> _deathMenu = default!;
  private Mock<IWinMenu> _winMenu = default!;
  private Mock<IPauseMenu> _pauseMenu = default!;

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
    _ui = new();
    _deathMenu = new();
    _winMenu = new();
    _pauseMenu = new();

    _logic.Setup(logic => logic.Bind()).Returns(_binding);

    _game = new() {
      IsTesting = true,
      GameRepo = _gameRepo.Object,
      GameLogic = _logic.Object,
      GameBinding = _binding,
      PlayerCamera = _playerCam.Object,
      Player = _player.Object,
      Map = _map.Object,
      InGameUi = _ui.Object,
      DeathMenu = _deathMenu.Object,
      WinMenu = _winMenu.Object,
      PauseMenu = _pauseMenu.Object
    };

    _game.FakeDependency(_appRepo.Object);
    _game.FakeNodeTree(new() {
      ["%PlayerCamera"] = _playerCam.Object,
      ["%Player"] = _player.Object,
      ["%Map"] = _map.Object,
      ["%InGameUi"] = _ui.Object,
      ["%DeathMenu"] = _deathMenu.Object,
      ["%WinMenu"] = _winMenu.Object,
      ["%PauseMenu"] = _pauseMenu.Object
    });

    await _fixture.AddToRoot(_game);
  }

  [Cleanup]
  public async Task Cleanup() => await _fixture.Cleanup();

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
  public void StartsGame() {
    _logic.Setup(logic => logic.Input(It.IsAny<GameLogic.Input.Initialize>()));
    _game.OnResolved();
    _playerCam.Setup(cam => cam.UsePlayerCamera());

    _binding.Output(new GameLogic.Output.StartGame());

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

  [Test]
  public void CapturesMouse() {
    _game.OnResolved();

    _binding.Output(new GameLogic.Output.CaptureMouse(true));
    Input.MouseMode.ShouldBe(Input.MouseModeEnum.Captured);

    _binding.Output(new GameLogic.Output.CaptureMouse(false));
    Input.MouseMode.ShouldBe(Input.MouseModeEnum.Visible);
  }

  [Test]
  public void ShowsLostScreen() {
    _deathMenu.Setup(menu => menu.Show());
    _deathMenu.Setup(menu => menu.FadeIn());
    _deathMenu.Setup(menu => menu.Animate());

    _game.OnResolved();

    _binding.Output(new GameLogic.Output.ShowLostScreen());

    _deathMenu.VerifyAll();
  }

  [Test]
  public void ExitsLostScreen() {
    _game.OnResolved();

    _binding.Output(new GameLogic.Output.ExitLostScreen());

    _deathMenu.Verify(menu => menu.FadeOut());
  }

  [Test]
  public void ShowsPauseMenu() {
    _pauseMenu.Setup(menu => menu.Show());
    _pauseMenu.Setup(menu => menu.FadeIn());

    _game.OnResolved();

    _binding.Output(new GameLogic.Output.ShowPauseMenu());

    _pauseMenu.VerifyAll();
  }

  [Test]
  public void ShowsWonScreen() {
    _winMenu.Setup(menu => menu.Show());
    _winMenu.Setup(menu => menu.FadeIn());

    _game.OnResolved();

    _binding.Output(new GameLogic.Output.ShowWonScreen());

    _winMenu.VerifyAll();
  }

  [Test]
  public void ExitsWonScreen() {
    _game.OnResolved();

    _binding.Output(new GameLogic.Output.ExitWonScreen());

    _winMenu.Verify(menu => menu.FadeOut());
  }

  [Test]
  public void ExitsPauseMenu() {
    _game.OnResolved();

    _binding.Output(new GameLogic.Output.ExitPauseMenu());

    _pauseMenu.Verify(menu => menu.FadeOut());
  }

  [Test]
  public void HidesPauseMenu() {
    _game.OnResolved();

    _binding.Output(new GameLogic.Output.HidePauseMenu());

    _pauseMenu.Verify(menu => menu.Hide());
  }

  [Test]
  public void ShowsPauseSaveOverlay() {
    _game.OnResolved();

    _binding.Output(new GameLogic.Output.ShowPauseSaveOverlay());

    _pauseMenu.Verify(menu => menu.OnSaveStarted());
  }

  [Test]
  public void HidesPauseSaveOverlay() {
    _game.OnResolved();

    _binding.Output(new GameLogic.Output.HidePauseSaveOverlay());

    _pauseMenu.Verify(menu => menu.OnSaveFinished());
  }

  [Test]
  public void InputsPauseButtonPressed() {
    _logic.Setup(
      logic => logic.Input(It.IsAny<GameLogic.Input.PauseButtonPressed>())
    );
    Input.ActionPress("ui_cancel");

    _game._Input(default!);

    _logic.VerifyAll();
  }

  [Test]
  public void OnMainMenu() {
    _game.OnMainMenu();

    _logic.Verify(
      logic => logic.Input(It.IsAny<GameLogic.Input.GoToMainMenu>())
    );
  }

  [Test]
  public void OnResume() {
    _game.OnResume();

    _logic.Verify(
      logic => logic.Input(It.IsAny<GameLogic.Input.PauseButtonPressed>())
    );
  }

  [Test]
  public void OnStart() {
    _game.OnStart();

    _logic.Verify(logic => logic.Input(It.IsAny<GameLogic.Input.Start>()));
  }

  [Test]
  public void OnWinMenuTransitioned() {
    _game.OnWinMenuTransitioned();

    _logic.Verify(
      logic => logic.Input(It.IsAny<GameLogic.Input.WinMenuTransitioned>())
    );
  }

  [Test]
  public void OnPauseMenuTransitioned() {
    _game.OnPauseMenuTransitioned();

    _logic.Verify(
      logic => logic.Input(It.IsAny<GameLogic.Input.PauseMenuTransitioned>())
    );
  }

  [Test]
  public void OnPauseMenuSaveRequested() {
    _game.OnPauseMenuSaveRequested();

    _logic.Verify(
      logic => logic.Input(It.IsAny<GameLogic.Input.SaveRequested>())
    );
  }

  [Test]
  public void OnDeathMenuTransitioned() {
    _game.OnDeathMenuTransitioned();

    _logic.Verify(
      logic => logic.Input(It.IsAny<GameLogic.Input.DeathMenuTransitioned>())
    );
  }
}
