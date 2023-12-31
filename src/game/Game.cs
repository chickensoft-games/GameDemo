namespace GameDemo;

using System;
using Chickensoft.AutoInject;
using Chickensoft.GodotNodeInterfaces;
using Chickensoft.LogicBlocks;
using Chickensoft.PowerUps;
using Godot;
using SuperNodes.Types;

public interface IGame :
  INode3D, IProvide<IGameRepo>, IProvide<IGameSaveSystem>;

[SuperNode(typeof(Provider), typeof(Dependent), typeof(AutoNode))]
public partial class Game : Node3D, IGame {
  public override partial void _Notification(int what);

  #region State

  public IGameRepo GameRepo { get; set; } = default!;
  public IGameLogic GameLogic { get; set; } = default!;

  public Logic<GameLogic.IState, Func<object, GameLogic.IState>, GameLogic.IState, Action<GameLogic.IState?>>.IBinding
    GameBinding { get; set; } = default!;

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
  IGameSaveSystem IProvide<IGameSaveSystem>.Value() => GameSaveSystem;

  #endregion Provisions

  #region Save

  public IGameSaveSerializer GameSaveSerializer { get; set; } = default!;
  public IGameSaveSystem GameSaveSystem { get; set; } = default!;

  #endregion Save

  #region Dependencies

  [Dependency] public IAppRepo AppRepo => DependOn<IAppRepo>();

  #endregion Dependencies

  public void Setup() {
    GameRepo = new GameRepo();
    GameLogic = new GameLogic(GameRepo, AppRepo);
    GameSaveSerializer = new GameSaveSerializer();
    GameSaveSystem = new GameSaveSystem(GameSaveSerializer);

    DeathMenu.TryAgain += OnStart;
    DeathMenu.MainMenu += OnMainMenu;
    WinMenu.MainMenu += OnMainMenu;
    PauseMenu.MainMenu += OnMainMenu;
    PauseMenu.Resume += OnResume;
    PauseMenu.TransitionCompleted += PauseMenuTransitioned;
    PauseMenu.Save += PauseMenuSaveRequested;

    Provide();

    // Calling Provide() triggers the Setup/OnResolved on dependent
    // nodes who depend on the values we provide. This means that
    // all nodes registering save managers will have already registered
    // their relevant save managers by now. This is useful when restoring state
    // while loading an existing save file.
  }

  public void OnResolved() {
    GameBinding = GameLogic.Bind();
    GameBinding
      .Handle<GameLogic.Output.ChangeToThirdPersonCamera>(
        _ => PlayerCamera.UsePlayerCamera()
      )
      .Handle<GameLogic.Output.SetPauseMode>(
        output => GetTree().Paused = output.IsPaused
      )
      .Handle<GameLogic.Output.CaptureMouse>(
        output => Input.MouseMode = output.IsMouseCaptured
          ? Input.MouseModeEnum.Captured
          : Input.MouseModeEnum.Visible
      )
      .Handle<GameLogic.Output.ShowPlayerDied>(_ => {
        DeathMenu.Show();
        DeathMenu.Animate();
      })
      .Handle<GameLogic.Output.ShowPauseMenu>(_ => {
        InGameUi.Show();
        PauseMenu.Show();
        PauseMenu.FadeIn();
      })
      .Handle<GameLogic.Output.ShowPlayerWon>(_ => {
        WinMenu.Show();
      })
      .Handle<GameLogic.Output.HidePauseMenu>(_ => PauseMenu.FadeOut())
      .Handle<GameLogic.Output.DisablePauseMenu>(_ => PauseMenu.Hide())
      .Handle<GameLogic.Output.ShowPauseSaveOverlay>(
        _ => PauseMenu.OnSaveStarted()
      )
      .Handle<GameLogic.Output.HidePauseSaveOverlay>(
        _ => PauseMenu.OnSaveFinished()
      );

    // Trigger the first state's OnEnter callbacks so our bindings run.
    // Keeps everything in sync from the moment we start!
    GameLogic.Start();

    GameLogic.Input(
      new GameLogic.Input.Initialize(NumCoinsInWorld: Map.GetCoinCount())
    );
  }

  public override void _Input(InputEvent @event) {
    if (Input.IsActionJustPressed("ui_cancel")) {
      GameLogic.Input(new GameLogic.Input.PauseButtonPressed());
    }
  }

  public void OnMainMenu() => GameLogic.Input(new GameLogic.Input.GoToMainMenu());

  public void OnResume() =>
    GameLogic.Input(new GameLogic.Input.PauseButtonPressed());

  public void OnStart() =>
    GameLogic.Input(new GameLogic.Input.StartGame());

  public void PauseMenuTransitioned() =>
    GameLogic.Input(new GameLogic.Input.PauseMenuTransitioned());

  public void PauseMenuSaveRequested() =>
    GameLogic.Input(new GameLogic.Input.GameSaveRequested());

  public void HideMenus() {
    InGameUi.Hide();
    DeathMenu.Hide();
    PauseMenu.Hide();
    WinMenu.Hide();
  }

  public void OnExitTree() {
    DeathMenu.TryAgain -= OnStart;
    DeathMenu.MainMenu -= OnMainMenu;
    WinMenu.MainMenu -= OnMainMenu;
    PauseMenu.MainMenu -= OnMainMenu;
    PauseMenu.Resume -= OnResume;
    PauseMenu.TransitionCompleted -= PauseMenuTransitioned;

    GameLogic.Stop();
    GameBinding.Dispose();
    GameRepo.Dispose();
  }
}
