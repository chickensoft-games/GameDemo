namespace GameDemo;

using System.Threading.Tasks;
using Chickensoft.AutoInject;
using Chickensoft.Collections;
using Chickensoft.GodotNodeInterfaces;
using Chickensoft.Introspection;
using Chickensoft.SaveFileBuilder;
using Godot;

public interface IGame : INode3D
  , ISaveable<GameData>
  , IProvide<IGameRepo>
  , IProvide<EntityTable>;

[Meta(typeof(IAutoNode))]
public partial class Game : Node3D, IGame
{
  public override void _Notification(int what) => this.Notify(what);

  #region Save

  [Dependency]
  public ISaveFile SaveFile => this.DependOn<ISaveFile>();

  public EntityTable EntityTable { get; set; } = new();
  EntityTable IProvide<EntityTable>.Value() => EntityTable;

  public GameData Save() => new()
  {
    MapData = Map.Save(),
    PlayerData = Player.Save(),
    PlayerCameraData = PlayerCamera.Save(),
  };

  public void Load(in GameData data)
  {
    Map.Load(data.MapData);
    Player.Load(data.PlayerData);
    PlayerCamera.Load(data.PlayerCameraData);
  }

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
  }

  public void OnResolved()
  {
    GameBinding = GameLogic.Bind()
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
    await SaveFile.SaveAsync(Save());
    GameLogic.Input(new GameLogic.Input.SaveCompleted());
  }

  private void SetPauseMode(bool isPaused) => GetTree().Paused = isPaused;
}
