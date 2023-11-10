namespace GameDemo;

using Chickensoft.AutoInject;
using Chickensoft.GodotNodeInterfaces;
using Chickensoft.PowerUps;
using Godot;
using SuperNodes.Types;

public interface IGame : INode3D, IProvide<IGameRepo> { }

[SuperNode(typeof(Provider), typeof(Dependent), typeof(AutoNode))]
public partial class Game : Node3D, IGame {
  public override partial void _Notification(int what);

  #region State
  public IGameRepo GameRepo { get; set; } = default!;
  public IGameLogic GameLogic { get; set; } = default!;
  public GameLogic.IBinding GameBinding { get; set; } = default!;
  #endregion State

  #region Nodes
  [Node]
  public IPlayerCamera PlayerCamera { get; set; } = default!;

  [Node]
  public IPlayer Player { get; set; } = default!;

  [Node]
  public IMap Map { get; set; } = default!;
  #endregion Nodes

  #region Provisions
  IGameRepo IProvide<IGameRepo>.Value() => GameRepo;
  #endregion Provisions

  #region Dependencies
  [Dependency]
  public IAppRepo AppRepo => DependOn<IAppRepo>();
  #endregion Dependencies

  public void Setup() {
    GameRepo = new GameRepo();
    GameLogic = new GameLogic(AppRepo);

    Provide();
  }

  public void OnResolved() {
    GameBinding = GameLogic.Bind();
    GameBinding
      .Handle<GameLogic.Output.ChangeToThirdPersonCamera>(
        (output) => PlayerCamera.UsePlayerCamera()
      )
      .Handle<GameLogic.Output.SetPauseMode>(
        (output) => GetTree().Paused = output.IsPaused
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
