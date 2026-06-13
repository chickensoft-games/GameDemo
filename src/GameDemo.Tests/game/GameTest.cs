namespace GameDemo.Tests;

using System.Diagnostics.CodeAnalysis;
using System.IO.Abstractions;
using System.Text.Json;
using System.Threading.Tasks;
using Chickensoft.AutoInject;
using Chickensoft.Collections;
using Chickensoft.LogicBlocks;
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
[Collection("GodotHeadless")]
public class GameTest : IDisposable
{
  private readonly IAppRepo _appRepo = new AppRepo();
  private readonly IGameRepo _gameRepo = new GameRepo();
  private readonly Mock<IGameLogic> _logic = new();

  private readonly LogicBlock.FakeBinding _binding = LogicBlock.CreateFakeBinding();

  private readonly Mock<IPlayerCamera> _playerCam = new();
  private readonly Mock<IPlayer> _player = new();
  private readonly Mock<IMap> _map = new();
  private readonly Mock<IInGameUI> _ui = new();
  private readonly Mock<IDeathMenu> _deathMenu = new();
  private readonly Mock<IWinMenu> _winMenu = new();
  private readonly Mock<IPauseMenu> _pauseMenu = new();
  private readonly EntityTable _entityTable = new();
  private readonly Mock<ISaveChunk<GameData>> _gameChunk = new();
  private readonly Mock<ISaveFile<GameData>> _saveFile = new();
  private readonly Mock<IFileSystem> _fileSystem = new();
  private readonly JsonSerializerOptions _jsonOptions = new()
  {
    WriteIndented = true,
    TypeInfoResolver = new SerializableTypeResolver(),
    Converters = {
      new SerializableTypeConverter(new Blackboard())
    },
  };
  private const string SAVE_FILE_PATH = "/game.json";

  private readonly Game _game;
  private readonly GodotHeadlessFixture _godot;

  public GameTest(GodotHeadlessFixture godot)
  {
    GodotSerialization.Setup();

    _godot = godot;

    _logic.Setup(logic => logic.Bind()).Returns(_binding);

    _saveFile.Setup(file => file.Root).Returns(_gameChunk.Object);

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
      SaveFile = _saveFile.Object,
      GameChunk = _gameChunk.Object,
      FileSystem = _fileSystem.Object,
      JsonOptions = _jsonOptions,
      SaveFilePath = SAVE_FILE_PATH
    };

    (_game as IAutoInit).IsTesting = true;

    _game.FakeDependency(_appRepo);
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

    _godot.Tree.Root.AddChild(_game);
  }

  public void Dispose() => _game.QueueFree();

  [Fact]
  public void Initializes()
  {
    ((IProvide<IGameRepo>)_game).Value().ShouldBe(_gameRepo);
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

  [Fact]
  public void StartsGame()
  {
    _logic.Setup(logic => logic.Input(It.IsAny<GameLogicState.Input.Initialize>()));
    _game.OnResolved();
    _playerCam.Setup(cam => cam.UsePlayerCamera());

    _binding.Output(new GameLogicState.Output.StartGame());

    _logic.VerifyAll();
    _playerCam.VerifyAll();
  }

  [Fact]
  public void SetsPauseMode()
  {
    _game.OnResolved();
    var tree = _godot.Tree;
    tree.Paused.ShouldBeFalse();

    _binding.Output(new GameLogicState.Output.SetPauseMode(IsPaused: true));

    _godot.GodotInstance.Iteration();

    tree.Paused.ShouldBeTrue();
    tree.Paused = false;
  }

  [Fact]
  public void CapturesMouse()
  {
    _game.OnResolved();

    _binding.Output(new GameLogicState.Output.CaptureMouse(true));
    Input.MouseMode.ShouldBe(Input.MouseModeEnum.Captured);

    _binding.Output(new GameLogicState.Output.CaptureMouse(false));
    Input.MouseMode.ShouldBe(Input.MouseModeEnum.Visible);
  }

  [Fact]
  public void ShowsLostScreen()
  {
    _deathMenu.Setup(menu => menu.Show());
    _deathMenu.Setup(menu => menu.FadeIn());
    _deathMenu.Setup(menu => menu.Animate());

    _game.OnResolved();

    _binding.Output(new GameLogicState.Output.ShowLostScreen());

    _deathMenu.VerifyAll();
  }

  [Fact]
  public void ExitsLostScreen()
  {
    _game.OnResolved();

    _binding.Output(new GameLogicState.Output.ExitLostScreen());

    _deathMenu.Verify(menu => menu.FadeOut());
  }

  [Fact]
  public void ShowsPauseMenu()
  {
    _pauseMenu.Setup(menu => menu.Show());
    _pauseMenu.Setup(menu => menu.FadeIn());

    _game.OnResolved();

    _binding.Output(new GameLogicState.Output.ShowPauseMenu());

    _pauseMenu.VerifyAll();
  }

  [Fact]
  public void ShowsWonScreen()
  {
    _winMenu.Setup(menu => menu.Show());
    _winMenu.Setup(menu => menu.FadeIn());

    _game.OnResolved();

    _binding.Output(new GameLogicState.Output.ShowWonScreen());

    _winMenu.VerifyAll();
  }

  [Fact]
  public void ExitsWonScreen()
  {
    _game.OnResolved();

    _binding.Output(new GameLogicState.Output.ExitWonScreen());

    _winMenu.Verify(menu => menu.FadeOut());
  }

  [Fact]
  public void ExitsPauseMenu()
  {
    _game.OnResolved();

    _binding.Output(new GameLogicState.Output.ExitPauseMenu());

    _pauseMenu.Verify(menu => menu.FadeOut());
  }

  [Fact]
  public void HidesPauseMenu()
  {
    _game.OnResolved();

    _binding.Output(new GameLogicState.Output.HidePauseMenu());

    _pauseMenu.Verify(menu => menu.Hide());
  }

  [Fact]
  public void ShowsPauseSaveOverlay()
  {
    _game.OnResolved();

    _binding.Output(new GameLogicState.Output.ShowPauseSaveOverlay());

    _pauseMenu.Verify(menu => menu.OnSaveStarted());
  }

  [Fact]
  public void HidesPauseSaveOverlay()
  {
    _game.OnResolved();

    _binding.Output(new GameLogicState.Output.HidePauseSaveOverlay());

    _pauseMenu.Verify(menu => menu.OnSaveCompleted());
  }

  [Fact]
  public void SavesGame()
  {
    _game.SaveFile = _saveFile.Object;

    _saveFile.Setup(file => file.Save()).Returns(Task.CompletedTask);

    _binding.Output(new GameLogicState.Output.StartSaving());

    _godot.GodotInstance.Iteration();

    _logic
      .Verify(logic => logic.Input(It.IsAny<GameLogicState.Input.SaveCompleted>()));
  }

  [Fact]
  public void InputsPauseButtonPressed()
  {
    _logic.Setup(
      logic => logic.Input(It.IsAny<GameLogicState.Input.PauseButtonPressed>())
    );
    Input.ActionPress("ui_cancel");

    _game._Input(null!);

    _logic.VerifyAll();
  }

  [Fact]
  public void OnMainMenu()
  {
    _game.OnMainMenu();

    _logic.Verify(
      logic => logic.Input(It.IsAny<GameLogicState.Input.GoToMainMenu>())
    );
  }

  [Fact]
  public void OnResume()
  {
    _game.OnResume();

    _logic.Verify(
      logic => logic.Input(It.IsAny<GameLogicState.Input.PauseButtonPressed>())
    );
  }

  [Fact]
  public void OnStart()
  {
    _game.OnStart();

    _logic.Verify(logic => logic.Input(It.IsAny<GameLogicState.Input.Start>()));
  }

  [Fact]
  public void OnWinMenuTransitioned()
  {
    _game.OnWinMenuTransitioned();

    _logic.Verify(
      logic => logic.Input(It.IsAny<GameLogicState.Input.WinMenuTransitioned>())
    );
  }

  [Fact]
  public void OnPauseMenuTransitioned()
  {
    _game.OnPauseMenuTransitioned();

    _logic.Verify(
      logic => logic.Input(It.IsAny<GameLogicState.Input.PauseMenuTransitioned>())
    );
  }

  [Fact]
  public void OnPauseMenuSaveRequested()
  {
    _game.OnPauseMenuSaveRequested();

    _logic.Verify(
      logic => logic.Input(It.IsAny<GameLogicState.Input.SaveRequested>())
    );
  }

  [Fact]
  public void OnDeathMenuTransitioned()
  {
    _game.OnDeathMenuTransitioned();

    _logic.Verify(
      logic => logic.Input(It.IsAny<GameLogicState.Input.DeathMenuTransitioned>())
    );
  }

  [Fact]
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
      StateMachine = null!,
      Velocity = Vector3.Zero
    };

    var playerCameraData = new PlayerCameraData()
    {
      GlobalTransform = Transform3D.Identity,
      StateMachine = null!,
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

  [Fact]
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
      StateMachine = null!,
      Velocity = Vector3.Zero
    };

    var playerCameraData = new PlayerCameraData()
    {
      GlobalTransform = Transform3D.Identity,
      StateMachine = null!,
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

  [Fact]
  public async Task SaveFileDoesNothingIfSaveFileDoesNotExist()
  {
    var file = new Mock<IFile>();
    _fileSystem.Setup(fs => fs.File).Returns(file.Object);

    file.Setup(f => f.Exists(_game.SaveFilePath)).Returns(false);

    _game.OnResolved();

    (await _game.SaveFile.OnLoad())
      .ShouldBeNull();
  }

  [Fact]
  public async Task SavesFile()
  {
    var file = new Mock<IFile>();
    _fileSystem.Setup(fs => fs.File).Returns(file.Object);

    _game.OnResolved();

    file
      .Setup(f => f.WriteAllTextAsync(
        SAVE_FILE_PATH, It.IsAny<string>(), CancellationToken.None
      ))
      .Returns(Task.CompletedTask);

    await _game.SaveFile.OnSave(TestSaveData.GameData);

    file.VerifyAll();
  }

  [Fact]
  public async Task LoadsSaveFile()
  {
    var file = new Mock<IFile>();
    _fileSystem.Setup(fs => fs.File).Returns(file.Object);

    file.Setup(f => f.Exists(SAVE_FILE_PATH)).Returns(true);

    file.Setup(f => f.ReadAllTextAsync(SAVE_FILE_PATH, CancellationToken.None))
      .ReturnsAsync(
        JsonSerializer.Serialize(TestSaveData.GameData, _jsonOptions)
      );

    _game.OnResolved();

    var loadedData = await _game.SaveFile.OnLoad();

    loadedData.ShouldBe(TestSaveData.GameData);
  }

  [Fact]
  public void LoadExistingGameWorks()
  {
    _saveFile.Reset();
    _saveFile.Setup(s => s.Load()).Returns(Task.CompletedTask);

    _game.SaveFile = _saveFile.Object;

    _game.LoadExistingGame();

    _godot.GodotInstance.Iteration();

    _saveFile.VerifyAll();
  }
}
