namespace GameDemo.Tests;

using System.Diagnostics.CodeAnalysis;
using System.IO.Abstractions;
using System.Text.Json;
using System.Threading.Tasks;
using Chickensoft.AutoInject;
using Chickensoft.Collections;
using Chickensoft.GoDotTest;
using Chickensoft.GodotTestDriver;
using Chickensoft.GodotTestDriver.Util;
using Chickensoft.SaveFileBuilder;
using Chickensoft.Serialization;
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
  private EntityTable _entityTable = default!;
  private Mock<ISaveChunk<GameData>> _gameChunk = default!;
  private Mock<ISaveFile<GameData>> _saveFile = default!;
  private Mock<IFileSystem> _fileSystem = default!;
  private JsonSerializerOptions _jsonOptions = default!;
  private const string SAVE_FILE_PATH = "/game.json";

  private Game _game = default!;

  public GameTest(Node testScene) : base(testScene) { }

  [SetupAll]
  public void SetupAll() => GodotSerialization.Setup();

  [Setup]
  public async Task Setup()
  {
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
    _entityTable = new();
    _gameChunk = new();
    _saveFile = new();
    _fileSystem = new();
    _jsonOptions = new()
    {
      WriteIndented = true,
      TypeInfoResolver = new SerializableTypeResolver(),
      Converters = {
        new SerializableTypeConverter(new Blackboard())
      },
    };

    _logic.Setup(logic => logic.Bind()).Returns(_binding);

    _saveFile.Setup(file => file.Root).Returns(_gameChunk.Object);

    _game = new()
    {
      GameRepo = _gameRepo.Object,
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
      SaveFile = _saveFile.Object,
      GameChunk = _gameChunk.Object,
      FileSystem = _fileSystem.Object,
      JsonOptions = _jsonOptions,
      SaveFilePath = SAVE_FILE_PATH
    };

    (_game as IAutoInit).IsTesting = true;

    _game.FakeDependency(_appRepo.Object);
    _game.FakeDependency(_entityTable);

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
    ((IProvide<IGameRepo>)_game).Value().ShouldBe(_gameRepo.Object);
    ((IProvide<ISaveChunk<GameData>>)_game).Value().ShouldBe(_gameChunk.Object);
    ((IProvide<EntityTable>)_game).Value().ShouldBe(_entityTable);

    _game.Setup();

    _game.SaveFilePath.ShouldBeOfType<string>();

    _game.GameRepo.ShouldBeOfType<GameRepo>();
    _game.GameBinding.ShouldBe(_binding);

    _game.OnResolved();
    // Make sure the game provided its dependencies.
    (_game as IProvider).ProviderState.IsInitialized.ShouldBeTrue();
  }

  [Test]
  public void StartsGame()
  {
    _logic.Setup(logic => logic.Input(It.IsAny<GameLogic.Input.Initialize>()));
    _game.OnResolved();
    _playerCam.Setup(cam => cam.UsePlayerCamera());

    _binding.Output(new GameLogic.Output.StartGame());

    _logic.VerifyAll();
    _playerCam.VerifyAll();
  }

  [Test]
  public async Task SetsPauseMode()
  {
    _game.OnResolved();
    var tree = TestScene.GetTree();
    tree.Paused.ShouldBeFalse();

    _binding.Output(new GameLogic.Output.SetPauseMode(IsPaused: true));

    await tree.NextFrame();

    tree.Paused.ShouldBeTrue();
    tree.Paused = false;
  }

  [Test]
  public void CapturesMouse()
  {
    _game.OnResolved();

    _binding.Output(new GameLogic.Output.CaptureMouse(true));
    Input.MouseMode.ShouldBe(Input.MouseModeEnum.Captured);

    _binding.Output(new GameLogic.Output.CaptureMouse(false));
    Input.MouseMode.ShouldBe(Input.MouseModeEnum.Visible);
  }

  [Test]
  public void ShowsLostScreen()
  {
    _deathMenu.Setup(menu => menu.Show());
    _deathMenu.Setup(menu => menu.FadeIn());
    _deathMenu.Setup(menu => menu.Animate());

    _game.OnResolved();

    _binding.Output(new GameLogic.Output.ShowLostScreen());

    _deathMenu.VerifyAll();
  }

  [Test]
  public void ExitsLostScreen()
  {
    _game.OnResolved();

    _binding.Output(new GameLogic.Output.ExitLostScreen());

    _deathMenu.Verify(menu => menu.FadeOut());
  }

  [Test]
  public void ShowsPauseMenu()
  {
    _pauseMenu.Setup(menu => menu.Show());
    _pauseMenu.Setup(menu => menu.FadeIn());

    _game.OnResolved();

    _binding.Output(new GameLogic.Output.ShowPauseMenu());

    _pauseMenu.VerifyAll();
  }

  [Test]
  public void ShowsWonScreen()
  {
    _winMenu.Setup(menu => menu.Show());
    _winMenu.Setup(menu => menu.FadeIn());

    _game.OnResolved();

    _binding.Output(new GameLogic.Output.ShowWonScreen());

    _winMenu.VerifyAll();
  }

  [Test]
  public void ExitsWonScreen()
  {
    _game.OnResolved();

    _binding.Output(new GameLogic.Output.ExitWonScreen());

    _winMenu.Verify(menu => menu.FadeOut());
  }

  [Test]
  public void ExitsPauseMenu()
  {
    _game.OnResolved();

    _binding.Output(new GameLogic.Output.ExitPauseMenu());

    _pauseMenu.Verify(menu => menu.FadeOut());
  }

  [Test]
  public void HidesPauseMenu()
  {
    _game.OnResolved();

    _binding.Output(new GameLogic.Output.HidePauseMenu());

    _pauseMenu.Verify(menu => menu.Hide());
  }

  [Test]
  public void ShowsPauseSaveOverlay()
  {
    _game.OnResolved();

    _binding.Output(new GameLogic.Output.ShowPauseSaveOverlay());

    _pauseMenu.Verify(menu => menu.OnSaveStarted());
  }

  [Test]
  public void HidesPauseSaveOverlay()
  {
    _game.OnResolved();

    _binding.Output(new GameLogic.Output.HidePauseSaveOverlay());

    _pauseMenu.Verify(menu => menu.OnSaveCompleted());
  }

  [Test]
  public async Task SavesGame()
  {
    _game.SaveFile = _saveFile.Object;

    _saveFile.Setup(file => file.Save()).Returns(Task.CompletedTask);

    _binding.Output(new GameLogic.Output.StartSaving());

    await TestScene.ProcessFrame();

    _logic
      .Verify(logic => logic.Input(It.IsAny<GameLogic.Input.SaveCompleted>()));
  }

  [Test]
  public void InputsPauseButtonPressed()
  {
    _logic.Setup(
      logic => logic.Input(It.IsAny<GameLogic.Input.PauseButtonPressed>())
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
      logic => logic.Input(It.IsAny<GameLogic.Input.GoToMainMenu>())
    );
  }

  [Test]
  public void OnResume()
  {
    _game.OnResume();

    _logic.Verify(
      logic => logic.Input(It.IsAny<GameLogic.Input.PauseButtonPressed>())
    );
  }

  [Test]
  public void OnStart()
  {
    _game.OnStart();

    _logic.Verify(logic => logic.Input(It.IsAny<GameLogic.Input.Start>()));
  }

  [Test]
  public void OnWinMenuTransitioned()
  {
    _game.OnWinMenuTransitioned();

    _logic.Verify(
      logic => logic.Input(It.IsAny<GameLogic.Input.WinMenuTransitioned>())
    );
  }

  [Test]
  public void OnPauseMenuTransitioned()
  {
    _game.OnPauseMenuTransitioned();

    _logic.Verify(
      logic => logic.Input(It.IsAny<GameLogic.Input.PauseMenuTransitioned>())
    );
  }

  [Test]
  public void OnPauseMenuSaveRequested()
  {
    _game.OnPauseMenuSaveRequested();

    _logic.Verify(
      logic => logic.Input(It.IsAny<GameLogic.Input.SaveRequested>())
    );
  }

  [Test]
  public void OnDeathMenuTransitioned()
  {
    _game.OnDeathMenuTransitioned();

    _logic.Verify(
      logic => logic.Input(It.IsAny<GameLogic.Input.DeathMenuTransitioned>())
    );
  }

  [Test]
  public void SavesChunk()
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

    _gameChunk.Setup(c => c.GetChunkSaveData<MapData>()).Returns(mapData);
    _gameChunk.Setup(c => c.GetChunkSaveData<PlayerData>()).Returns(playerData);
    _gameChunk.Setup(c => c.GetChunkSaveData<PlayerCameraData>())
      .Returns(playerCameraData);

    // Call the real game chunk, but pass in the fake game chunk to its
    // handlers to make mocking the save data easier.
    var saveData = _game.GameChunk.OnSave(_gameChunk.Object);

    saveData.MapData.ShouldBe(mapData);
    saveData.PlayerData.ShouldBe(playerData);
    saveData.PlayerCameraData.ShouldBe(playerCameraData);
  }

  [Test]
  public void LoadsChunk()
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

    _gameChunk.Setup(c => c.LoadChunkSaveData(mapData));
    _gameChunk.Setup(c => c.LoadChunkSaveData(playerData));
    _gameChunk.Setup(c => c.LoadChunkSaveData(playerCameraData));

    _game.GameChunk.OnLoad(_gameChunk.Object, gameData);

    _gameChunk.VerifyAll();
  }

  [Test]
  public async Task SaveFileDoesNothingIfSaveFileDoesNotExist()
  {
    var file = new Mock<IFile>();
    _fileSystem.Setup(fs => fs.File).Returns(file.Object);

    file.Setup(f => f.Exists(_game.SaveFilePath)).Returns(false);

    _game.OnResolved();

    (await _game.SaveFile.OnLoad())
      .ShouldBeNull();
  }

  [Test]
  public async Task SavesFile()
  {
    var file = new Mock<IFile>();
    _fileSystem.Setup(fs => fs.File).Returns(file.Object);

    _game.OnResolved();

    file
      .Setup(f => f.WriteAllTextAsync(
        SAVE_FILE_PATH, It.IsAny<string>(), default
      ))
      .Returns(Task.CompletedTask);

    await _game.SaveFile.OnSave(TestSaveData.GameData);

    file.VerifyAll();
  }

  [Test]
  public async Task LoadsSaveFile()
  {
    var file = new Mock<IFile>();
    _fileSystem.Setup(fs => fs.File).Returns(file.Object);

    file.Setup(f => f.Exists(SAVE_FILE_PATH)).Returns(true);

    file.Setup(f => f.ReadAllTextAsync(SAVE_FILE_PATH, default))
      .ReturnsAsync(
        JsonSerializer.Serialize(TestSaveData.GameData, _jsonOptions)
      );

    _game.OnResolved();

    var loadedData = await _game.SaveFile.OnLoad();

    loadedData.ShouldBe(TestSaveData.GameData);
  }

  [Test]
  public async Task LoadExistingGameWorks()
  {
    _saveFile.Reset();
    _saveFile.Setup(s => s.Load()).Returns(Task.CompletedTask);

    _game.SaveFile = _saveFile.Object;

    _game.LoadExistingGame();

    await TestScene.ProcessFrame();

    _saveFile.VerifyAll();
  }
}
