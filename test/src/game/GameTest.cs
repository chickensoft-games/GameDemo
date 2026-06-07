namespace GameDemo.Tests;

using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Chickensoft.AutoInject;
using Chickensoft.Collections;
using Chickensoft.GoDotTest;
using Chickensoft.GodotTestDriver;
using Chickensoft.GodotTestDriver.Util;
using Chickensoft.LogicBlocks;
using Chickensoft.SaveFileBuilder;
using Chickensoft.Serialization.Godot;
using Godot;
using Moq;
using Shouldly;

[
  SuppressMessage(
    "Design",
    "CA1001",
    Justification = "Disposable field is added to TestDriver fixture"
  )
]
public class GameTest : TestClass
{
  private Fixture _fixture = default!;
  private IAppRepo _appRepo = default!;
  private IGameRepo _gameRepo = default!;
  private Mock<IGameLogic> _logic = default!;

  private LogicBlock.FakeBinding _binding = default!;

  private Mock<IPlayerCamera> _playerCam = default!;
  private Mock<IPlayer> _player = default!;
  private Mock<IMap> _map = default!;
  private Mock<IInGameUI> _ui = default!;
  private Mock<IDeathMenu> _deathMenu = default!;
  private Mock<IWinMenu> _winMenu = default!;
  private Mock<IPauseMenu> _pauseMenu = default!;
  private EntityTable _entityTable = default!;
  private Mock<ISaveFile> _saveFile = default!;

  private Game _game = default!;

  public GameTest(Node testScene) : base(testScene) { }

  [SetupAll]
  public void SetupAll() => GodotSerialization.Setup();

  [Setup]
  public async Task Setup()
  {
    _fixture = new(TestScene.GetTree());

    _appRepo = new AppRepo();
    _gameRepo = new GameRepo();
    _logic = new();
    _binding = LogicBlock.CreateFakeBinding();
    _playerCam = new();
    _player = new();
    _map = new();
    _ui = new();
    _deathMenu = new();
    _winMenu = new();
    _pauseMenu = new();
    _entityTable = new();
    _saveFile = new();

    _logic.Setup(logic => logic.Bind()).Returns(_binding);

    _game = new()
    {
      GameRepo = _gameRepo,
      GameLogic = _logic.Object,
      GameBinding = _binding,
      PlayerCamera = _playerCam.Object,
      Player = _player.Object,
      Map = _map.Object,
      InGameUi = _ui.Object,
      DeathMenu = _deathMenu.Object,
      WinMenu = _winMenu.Object,
      PauseMenu = _pauseMenu.Object,
      EntityTable = _entityTable,
    };

    (_game as IAutoInit).IsTesting = true;

    _game.FakeDependency(_appRepo);
    _game.FakeDependency(_entityTable);
    _game.FakeDependency(_saveFile.Object);

    _game.FakeNodeTree(new()
    {
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
  public void Initializes()
  {
    ((IProvide<IGameRepo>)_game).Value().ShouldBe(_gameRepo);
    ((IProvide<EntityTable>)_game).Value().ShouldBe(_entityTable);

    _game.Setup();

    _game.GameRepo.ShouldBeOfType<GameRepo>();
    _game.GameBinding.ShouldBe(_binding);

    _game.OnResolved();
    // Make sure the game provided its dependencies.
    (_game as IProvider).ProviderState.IsInitialized.ShouldBeTrue();
  }

  [Test]
  public void StartsGame()
  {
    _logic.Setup(logic => logic.Input(It.IsAny<GameLogicState.Input.Initialize>()));
    _game.OnResolved();
    _playerCam.Setup(cam => cam.UsePlayerCamera());

    _binding.Output(new GameLogicState.Output.StartGame());

    _logic.VerifyAll();
    _playerCam.VerifyAll();
  }

  [Test]
  public async Task SetsPauseMode()
  {
    _game.OnResolved();
    var tree = TestScene.GetTree();
    tree.Paused.ShouldBeFalse();

    _binding.Output(new GameLogicState.Output.SetPauseMode(IsPaused: true));

    await tree.NextFrame();

    tree.Paused.ShouldBeTrue();
    tree.Paused = false;
  }

  [Test]
  public void CapturesMouse()
  {
    _game.OnResolved();

    _binding.Output(new GameLogicState.Output.CaptureMouse(true));
    Input.MouseMode.ShouldBe(Input.MouseModeEnum.Captured);

    _binding.Output(new GameLogicState.Output.CaptureMouse(false));
    Input.MouseMode.ShouldBe(Input.MouseModeEnum.Visible);
  }

  [Test]
  public void ShowsLostScreen()
  {
    _deathMenu.Setup(menu => menu.Show());
    _deathMenu.Setup(menu => menu.FadeIn());
    _deathMenu.Setup(menu => menu.Animate());

    _game.OnResolved();

    _binding.Output(new GameLogicState.Output.ShowLostScreen());

    _deathMenu.VerifyAll();
  }

  [Test]
  public void ExitsLostScreen()
  {
    _game.OnResolved();

    _binding.Output(new GameLogicState.Output.ExitLostScreen());

    _deathMenu.Verify(menu => menu.FadeOut());
  }

  [Test]
  public void ShowsPauseMenu()
  {
    _pauseMenu.Setup(menu => menu.Show());
    _pauseMenu.Setup(menu => menu.FadeIn());

    _game.OnResolved();

    _binding.Output(new GameLogicState.Output.ShowPauseMenu());

    _pauseMenu.VerifyAll();
  }

  [Test]
  public void ShowsWonScreen()
  {
    _winMenu.Setup(menu => menu.Show());
    _winMenu.Setup(menu => menu.FadeIn());

    _game.OnResolved();

    _binding.Output(new GameLogicState.Output.ShowWonScreen());

    _winMenu.VerifyAll();
  }

  [Test]
  public void ExitsWonScreen()
  {
    _game.OnResolved();

    _binding.Output(new GameLogicState.Output.ExitWonScreen());

    _winMenu.Verify(menu => menu.FadeOut());
  }

  [Test]
  public void ExitsPauseMenu()
  {
    _game.OnResolved();

    _binding.Output(new GameLogicState.Output.ExitPauseMenu());

    _pauseMenu.Verify(menu => menu.FadeOut());
  }

  [Test]
  public void HidesPauseMenu()
  {
    _game.OnResolved();

    _binding.Output(new GameLogicState.Output.HidePauseMenu());

    _pauseMenu.Verify(menu => menu.Hide());
  }

  [Test]
  public void ShowsPauseSaveOverlay()
  {
    _game.OnResolved();

    _binding.Output(new GameLogicState.Output.ShowPauseSaveOverlay());

    _pauseMenu.Verify(menu => menu.OnSaveStarted());
  }

  [Test]
  public void HidesPauseSaveOverlay()
  {
    _game.OnResolved();

    _binding.Output(new GameLogicState.Output.HidePauseSaveOverlay());

    _pauseMenu.Verify(menu => menu.OnSaveCompleted());
  }

  [Test]
  public void SavesGame()
  {
    _saveFile.Setup(file => file.SaveAsync(It.IsAny<GameData>())).Returns(ValueTask.CompletedTask);

    _binding.Output(new GameLogicState.Output.StartSaving());

    _saveFile.VerifyAll();
    _logic.Verify(logic => logic.Input(It.IsAny<GameLogicState.Input.SaveCompleted>()));
  }

  [Test]
  public void InputsPauseButtonPressed()
  {
    _logic.Setup(
      logic => logic.Input(It.IsAny<GameLogicState.Input.PauseButtonPressed>())
    );
    Input.ActionPress("ui_cancel");

    _game._Input(default!);

    _logic.VerifyAll();
  }

  [Test]
  public void OnMainMenu()
  {
    _game.OnMainMenu();

    _logic.Verify(
      logic => logic.Input(It.IsAny<GameLogicState.Input.GoToMainMenu>())
    );
  }

  [Test]
  public void OnResume()
  {
    _game.OnResume();

    _logic.Verify(
      logic => logic.Input(It.IsAny<GameLogicState.Input.PauseButtonPressed>())
    );
  }

  [Test]
  public void OnStart()
  {
    _game.OnStart();

    _logic.Verify(logic => logic.Input(It.IsAny<GameLogicState.Input.Start>()));
  }

  [Test]
  public void OnWinMenuTransitioned()
  {
    _game.OnWinMenuTransitioned();

    _logic.Verify(
      logic => logic.Input(It.IsAny<GameLogicState.Input.WinMenuTransitioned>())
    );
  }

  [Test]
  public void OnPauseMenuTransitioned()
  {
    _game.OnPauseMenuTransitioned();

    _logic.Verify(
      logic => logic.Input(It.IsAny<GameLogicState.Input.PauseMenuTransitioned>())
    );
  }

  [Test]
  public void OnPauseMenuSaveRequested()
  {
    _game.OnPauseMenuSaveRequested();

    _logic.Verify(
      logic => logic.Input(It.IsAny<GameLogicState.Input.SaveRequested>())
    );
  }

  [Test]
  public void OnDeathMenuTransitioned()
  {
    _game.OnDeathMenuTransitioned();

    _logic.Verify(
      logic => logic.Input(It.IsAny<GameLogicState.Input.DeathMenuTransitioned>())
    );
  }

  [Test]
  public void Saves()
  {
    _game.Setup();

    var mapData = new MapData()
    {
      CoinsBeingCollected = [],
      CollectedCoinIds = []
    };

    var playerData = new PlayerData()
    {
      GlobalTransform = Transform3D.Identity,
      StateMachine = default!,
      Velocity = Vector3.Zero
    };

    var playerCameraData = new PlayerCameraData()
    {
      GlobalTransform = Transform3D.Identity,
      StateMachine = default!,
      LocalPosition = Vector3.Zero,
      OffsetPosition = Vector3.Zero
    };

    _map.Setup(m => m.Save()).Returns(mapData);
    _player.Setup(p => p.Save()).Returns(playerData);
    _playerCam.Setup(p => p.Save()).Returns(playerCameraData);

    // Call the real game chunk, but pass in the fake game chunk to its
    // handlers to make mocking the save data easier.
    var saveData = _game.Save();

    saveData.MapData.ShouldBe(mapData);
    saveData.PlayerData.ShouldBe(playerData);
    saveData.PlayerCameraData.ShouldBe(playerCameraData);
  }

  [Test]
  public void Loads()
  {
    _game.Setup();

    var mapData = new MapData()
    {
      CoinsBeingCollected = [],
      CollectedCoinIds = []
    };

    var playerData = new PlayerData()
    {
      GlobalTransform = Transform3D.Identity,
      StateMachine = default!,
      Velocity = Vector3.Zero
    };

    var playerCameraData = new PlayerCameraData()
    {
      GlobalTransform = Transform3D.Identity,
      StateMachine = default!,
      LocalPosition = Vector3.Zero,
      OffsetPosition = Vector3.Zero
    };

    var gameData = new GameData()
    {
      MapData = mapData,
      PlayerData = playerData,
      PlayerCameraData = playerCameraData
    };

    _game.Load(gameData);

    _map.Verify(m => m.Load(mapData));
    _player.Verify(p => p.Load(playerData));
    _playerCam.Verify(p => p.Load(playerCameraData));
  }
}
