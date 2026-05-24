namespace GameDemo;

using System;
using System.IO.Abstractions;
using System.Text.Json;
using Chickensoft.AutoInject;
using Chickensoft.Collections;
using Chickensoft.GodotNodeInterfaces;
using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;
using Chickensoft.LogicBlocks.Auto;
using Chickensoft.SaveFileBuilder;
using Chickensoft.Serialization;
using Chickensoft.Serialization.Godot;
using Godot;

public interface IGame : INode3D,
IProvide<IGameRepo>, IProvide<ISaveChunk<GameData>>, IProvide<EntityTable>
{
  void LoadExistingGame();

  event Game.SaveFileLoadedEventHandler? SaveFileLoaded;
}

[Meta(typeof(IAutoNode))]
public partial class Game : Node3D, IGame
{
  public override void _Notification(int what) => this.Notify(what);

  #region Save
  [Signal]
  public delegate void SaveFileLoadedEventHandler();
  public JsonSerializerOptions JsonOptions { get; set; } = default!;
  public const string SAVE_FILE_NAME = "game.json";
  public IFileSystem FileSystem { get; set; } = default!;
  public IEnvironmentProvider Environment { get; set; } = default!;
  public string SaveFilePath { get; set; } = default!;
  public EntityTable EntityTable { get; set; } = new();
  EntityTable IProvide<EntityTable>.Value() => EntityTable;
  public ISaveFile<GameData> SaveFile { get; set; } = default!;
  public ISaveChunk<GameData> GameChunk { get; set; } = default!;
  ISaveChunk<GameData> IProvide<ISaveChunk<GameData>>.Value() => GameChunk;
  #endregion Save

  #region State

  public IGameRepo GameRepo { get; set; } = default!;
  public IGameLogic GameLogic { get; set; } = default!;

  public LogicBlock.Binding GameBinding { get; set; } = default!;

  #endregion State

  #region Nodes

  [Node] public IPlayerCamera PlayerCamera { get; set; } = default!;

  [Node] public IPlayer Player { get; set; } = default!;

  [Node] public IMap Map { get; set; } = default!;
  [Node] public IInGameUI InGameUi { get; set; } = default!;
  [Node] public IDeathMenu DeathMenu { get; set; } = default!;
  [Node] public IWinMenu WinMenu { get; set; } = default!;
  [Node] public IPauseMenu PauseMenu { get; set; } = default!;

  #endregion Nodes

  #region Provisions

  IGameRepo IProvide<IGameRepo>.Value() => GameRepo;

  #endregion Provisions

  #region Dependencies

  [Dependency] public IAppRepo AppRepo => this.DependOn<IAppRepo>();

  #endregion Dependencies

  public void Setup()
  {
    FileSystem = new FileSystem();

    SaveFilePath = FileSystem.Path.Join(OS.GetUserDataDir(), SAVE_FILE_NAME);

    GameRepo = new GameRepo();
    GameLogic = new GameLogic();
    GameLogic.Set(GameRepo);
    GameLogic.Set(AppRepo);

    // Tell our type resolver about the Godot-specific & LogicBlock-specific converters.
    GodotSerialization.Setup();
    LogicBlockSerialization.Setup();

    // Create a standard JsonSerializerOptions with our introspective type
    // resolver and the logic blocks converter.
    JsonOptions = new JsonSerializerOptions
    {
      Converters = {
        new SerializableTypeConverter()
      },
      TypeInfoResolver = new SerializableTypeResolver(),
      WriteIndented = true
    };

    DeathMenu.TryAgain += OnStart;
    DeathMenu.MainMenu += OnMainMenu;
    DeathMenu.TransitionCompleted += OnDeathMenuTransitioned;

    WinMenu.MainMenu += OnMainMenu;
    WinMenu.TransitionCompleted += OnWinMenuTransitioned;

    PauseMenu.MainMenu += OnMainMenu;
    PauseMenu.Resume += OnResume;
    PauseMenu.TransitionCompleted += OnPauseMenuTransitioned;
    PauseMenu.Save += OnPauseMenuSaveRequested;

    GameChunk = new SaveChunk<GameData>(
      (chunk) =>
      {
        var gameData = new GameData()
        {
          MapData = chunk.GetChunkSaveData<MapData>(),
          PlayerData = chunk.GetChunkSaveData<PlayerData>(),
          PlayerCameraData = chunk.GetChunkSaveData<PlayerCameraData>()
        };

        return gameData;
      },
        onLoad: (chunk, data) =>
        {
          chunk.LoadChunkSaveData(data.MapData);
          chunk.LoadChunkSaveData(data.PlayerData);
          chunk.LoadChunkSaveData(data.PlayerCameraData);
        }
      );

    // Calling Provide() triggers the Setup/OnResolved on dependent
    // nodes who depend on the values we provide. This means that
    // all nodes registering save managers will have already registered
    // their relevant save managers by now. This is useful when restoring state
    // while loading an existing save file.
  }

  public void OnResolved()
  {
    SaveFile = new SaveFile<GameData>(
      root: GameChunk,
      onSave: async data =>
      {
        // Save the game data to disk.
        var json = JsonSerializer.Serialize(data, JsonOptions);
        await FileSystem.File.WriteAllTextAsync(SaveFilePath, json);
      },
      onLoad: async () =>
      {
        // Load the game data from disk.
        if (!FileSystem.File.Exists(SaveFilePath))
        {
          GD.Print("No save file to load :'(");
          return null;
        }

        var json = await FileSystem.File.ReadAllTextAsync(SaveFilePath);
        return JsonSerializer.Deserialize<GameData>(json, JsonOptions);
      }
    );

    GameBinding = GameLogic.Bind();
    GameBinding
      .OnOutput(
        (in GameLogicState.Output.StartGame _) =>
        {
          PlayerCamera.UsePlayerCamera();
          InGameUi.Show();
        })
      .OnOutput(
        (in GameLogicState.Output.SetPauseMode output) =>
          CallDeferred(nameof(SetPauseMode), output.IsPaused)
      )
      .OnOutput(
        (in GameLogicState.Output.CaptureMouse output) =>
          Input.MouseMode = output.IsMouseCaptured
            ? Input.MouseModeEnum.Captured
            : Input.MouseModeEnum.Visible
      )
      .OnOutput((in GameLogicState.Output.ShowLostScreen _) =>
      {
        DeathMenu.Show();
        DeathMenu.FadeIn();
        DeathMenu.Animate();
      })
      .OnOutput((in GameLogicState.Output.ExitLostScreen _) => DeathMenu.FadeOut())
      .OnOutput((in GameLogicState.Output.ShowPauseMenu _) =>
      {
        PauseMenu.Show();
        PauseMenu.FadeIn();
      })
      .OnOutput((in GameLogicState.Output.ShowWonScreen _) =>
      {
        WinMenu.Show();
        WinMenu.FadeIn();
      })
      .OnOutput((in GameLogicState.Output.ExitWonScreen _) => WinMenu.FadeOut())
      .OnOutput((in GameLogicState.Output.ExitPauseMenu _) => PauseMenu.FadeOut())
      .OnOutput((in GameLogicState.Output.HidePauseMenu _) => PauseMenu.Hide())
      .OnOutput((in GameLogicState.Output.ShowPauseSaveOverlay _) =>
        PauseMenu.OnSaveStarted()
      )
      .OnOutput((in GameLogicState.Output.HidePauseSaveOverlay _) =>
        PauseMenu.OnSaveCompleted()
      )
      .OnOutput((in GameLogicState.Output.StartSaving _) =>
        SaveFile.Save().ContinueWith(
          // Saving is async. The game node is always around, so kicking off
          // an async process is safe. Plus, we block input while saving, so
          // no interruptions.
          (task) => GameLogic.Input(new GameLogicState.Input.SaveCompleted())
        )
      );

    // Trigger the first state's OnEnter callbacks so our bindings run.
    // Keeps everything in sync from the moment we start!
    GameLogic.Start();

    GameLogic.Input(
      new GameLogicState.Input.Initialize(NumCoinsInWorld: Map.GetCoinCount())
    );

    this.Provide();
  }

  public override void _Input(InputEvent @event)
  {
    if (Input.IsActionJustPressed("ui_cancel"))
    {
      GameLogic.Input(new GameLogicState.Input.PauseButtonPressed());
    }
  }

  public void OnMainMenu() =>
    GameLogic.Input(new GameLogicState.Input.GoToMainMenu());

  public void OnResume() =>
    GameLogic.Input(new GameLogicState.Input.PauseButtonPressed());

  public void OnStart() =>
    GameLogic.Input(new GameLogicState.Input.Start());

  public void OnWinMenuTransitioned() =>
    GameLogic.Input(new GameLogicState.Input.WinMenuTransitioned());

  public void OnPauseMenuTransitioned() =>
    GameLogic.Input(new GameLogicState.Input.PauseMenuTransitioned());

  public void OnPauseMenuSaveRequested() =>
    GameLogic.Input(new GameLogicState.Input.SaveRequested());

  public void OnDeathMenuTransitioned() =>
    GameLogic.Input(new GameLogicState.Input.DeathMenuTransitioned());

  public void OnExitTree()
  {
    DeathMenu.TryAgain -= OnStart;
    DeathMenu.MainMenu -= OnMainMenu;
    DeathMenu.TransitionCompleted -= OnDeathMenuTransitioned;
    WinMenu.MainMenu -= OnMainMenu;
    PauseMenu.MainMenu -= OnMainMenu;
    PauseMenu.Resume -= OnResume;
    PauseMenu.TransitionCompleted -= OnPauseMenuTransitioned;

    GameLogic.Stop();
    GameBinding.Dispose();
    GameRepo.Dispose();
  }

  public void LoadExistingGame()
  {
    SaveFile.Load()
      .ContinueWith((_) => CallDeferred(nameof(FinishedLoadingSaveFile)));
  }

  private void FinishedLoadingSaveFile()
    => EmitSignal(SignalName.SaveFileLoaded);

  private void SetPauseMode(bool isPaused) => GetTree().Paused = isPaused;
}
