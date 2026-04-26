namespace GameDemo;

using System;
using System.Threading.Tasks;
using Chickensoft.AutoInject;
using Chickensoft.Collections;
using Chickensoft.GodotNodeInterfaces;
using Chickensoft.Introspection;
using Chickensoft.SaveFileBuilder;
using Godot;

public interface IGame : INode3D,
IProvide<IGameRepo>, IProvide<ISaveChunk<GameData>>, IProvide<EntityTable>
{
  ValueTask LoadExistingGame();

  event Game.SaveFileLoadedEventHandler? SaveFileLoaded;
}

[Meta(typeof(IAutoNode))]
public partial class Game : Node3D, IGame
{
  public override void _Notification(int what) => this.Notify(what);

  #region Save

  [Signal]
  public delegate void SaveFileLoadedEventHandler();

  [Dependency]
  public ISaveFile SaveFile => this.DependOn<ISaveFile>();

  public IEnvironmentProvider Environment { get; set; } = default!;
  public string SaveFilePath { get; set; } = default!;
  public EntityTable EntityTable { get; set; } = new();
  EntityTable IProvide<EntityTable>.Value() => EntityTable;
  public ISaveChunk<GameData> GameChunk { get; set; } = default!;
  ISaveChunk<GameData> IProvide<ISaveChunk<GameData>>.Value() => GameChunk;

  #endregion Save

  #region State

  public IGameRepo GameRepo { get; set; } = default!;
  public IGameLogic GameLogic { get; set; } = default!;

  public GameLogic.IBinding GameBinding { get; set; } = default!;

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
    GameRepo = new GameRepo();
    GameLogic = new GameLogic();
    GameLogic.Set(GameRepo);
    GameLogic.Set(AppRepo);

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
      (chunk) => new GameData()
      {
        MapData = chunk.GetChunkSaveData<MapData>(),
        PlayerData = chunk.GetChunkSaveData<PlayerData>(),
        PlayerCameraData = chunk.GetChunkSaveData<PlayerCameraData>()
      },
      onLoad: (chunk, data) =>
      {
        chunk.LoadChunkSaveData(data.MapData);
        chunk.LoadChunkSaveData(data.PlayerData);
        chunk.LoadChunkSaveData(data.PlayerCameraData);
      });
  }

  public void OnResolved()
  {
    GameBinding = GameLogic.Bind();
    GameBinding
      .Handle((in GameLogic.Output.StartGame _) =>
      {
        PlayerCamera.UsePlayerCamera();
        InGameUi.Show();
      })
      .Handle((in GameLogic.Output.SetPauseMode output) =>
        CallDeferred(nameof(SetPauseMode), output.IsPaused)
      )
      .Handle((in GameLogic.Output.CaptureMouse output) =>
        Input.MouseMode = output.IsMouseCaptured
          ? Input.MouseModeEnum.Captured
          : Input.MouseModeEnum.Visible
      )
      .Handle((in GameLogic.Output.ShowLostScreen _) =>
      {
        DeathMenu.Show();
        DeathMenu.FadeIn();
        DeathMenu.Animate();
      })
      .Handle((in GameLogic.Output.ExitLostScreen _) => DeathMenu.FadeOut())
      .Handle((in GameLogic.Output.ShowPauseMenu _) =>
      {
        PauseMenu.Show();
        PauseMenu.FadeIn();
      })
      .Handle((in GameLogic.Output.ShowWonScreen _) =>
      {
        WinMenu.Show();
        WinMenu.FadeIn();
      })
      .Handle((in GameLogic.Output.ExitWonScreen _) => WinMenu.FadeOut())
      .Handle((in GameLogic.Output.ExitPauseMenu _) => PauseMenu.FadeOut())
      .Handle((in GameLogic.Output.HidePauseMenu _) => PauseMenu.Hide())
      .Handle((in GameLogic.Output.ShowPauseSaveOverlay _) => PauseMenu.OnSaveStarted())
      .Handle((in GameLogic.Output.HidePauseSaveOverlay _) => PauseMenu.OnSaveCompleted())
      .Handle((in GameLogic.Output.StartSaving _) => SaveGame().AsTask());

    // Trigger the first state's OnEnter callbacks so our bindings run.
    // Keeps everything in sync from the moment we start!
    GameLogic.Start();

    GameLogic.Input(new GameLogic.Input.Initialize(NumCoinsInWorld: Map.GetCoinCount()));

    // Calling Provide() triggers the Setup/OnResolved on dependent
    // nodes who depend on the values we provide. This means that
    // all nodes registering save managers will have already registered
    // their relevant save managers by now. This is useful when restoring state
    // while loading an existing save file.
    this.Provide();
  }

  public override void _Input(InputEvent @event)
  {
    if (Input.IsActionJustPressed("ui_cancel"))
    {
      GameLogic.Input(new GameLogic.Input.PauseButtonPressed());
    }
  }

  public void OnMainMenu() =>
    GameLogic.Input(new GameLogic.Input.GoToMainMenu());

  public void OnResume() =>
    GameLogic.Input(new GameLogic.Input.PauseButtonPressed());

  public void OnStart() =>
    GameLogic.Input(new GameLogic.Input.Start());

  public void OnWinMenuTransitioned() =>
    GameLogic.Input(new GameLogic.Input.WinMenuTransitioned());

  public void OnPauseMenuTransitioned() =>
    GameLogic.Input(new GameLogic.Input.PauseMenuTransitioned());

  public void OnPauseMenuSaveRequested() =>
    GameLogic.Input(new GameLogic.Input.SaveRequested());

  public void OnDeathMenuTransitioned() =>
    GameLogic.Input(new GameLogic.Input.DeathMenuTransitioned());

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

  private async ValueTask SaveGame()
  {
    await SaveFile.SaveAsync(GameChunk.GetSaveData());
    GameLogic.Input(new GameLogic.Input.SaveCompleted());
  }

  public async ValueTask LoadExistingGame()
  {
    if (await SaveFile.ExistsAsync()
      && await SaveFile.LoadAsync<GameData>() is { } data)
    {
      GameChunk.LoadSaveData(data);
    }

    EmitSignal(SignalName.SaveFileLoaded);
  }

  private void SetPauseMode(bool isPaused) => GetTree().Paused = isPaused;
}
