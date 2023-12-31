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
    GameLogic = new GameLogic(AppRepo);
    GameSaveSerializer = new GameSaveSerializer();
    GameSaveSystem = new GameSaveSystem(GameSaveSerializer);


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
      );

    // Trigger the first state's OnEnter callbacks so our bindings run.
    // Keeps everything in sync from the moment we start!
    GameLogic.Start();

    GameLogic.Input(
      new GameLogic.Input.Initialize(NumCoinsInWorld: Map.GetCoinCount())
    );
  }

  public void OnExitTree() {
    GameLogic.Stop();
    GameBinding.Dispose();
    GameRepo.Dispose();
  }
}
