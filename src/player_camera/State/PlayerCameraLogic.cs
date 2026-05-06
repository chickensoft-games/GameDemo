namespace GameDemo;

using System;
using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;
using Chickensoft.LogicBlocks.Auto;
using Chickensoft.Serialization;
using Chickensoft.Sync.Primitives;
using Godot;

public interface IPlayerCameraLogic : IAutoLogicBlock;

[Meta, Id("player_camera_logic")]
public partial class PlayerCameraLogic : AutoBlock, IPlayerCameraLogic
{
  private AutoValue<bool>.Binding? _isMouseCapturedBinding;

  private AutoValue<Vector3>.Binding? _playerGlobalPositionBinding;

  public override Type GetInitialState() => typeof(BaseState.InputDisabled);

  public PlayerCameraLogic()
  {
    Preallocate<BaseState>();
  }

  public override void OnStart()
  {
    var gameRepo = Get<IGameRepo>();
    _isMouseCapturedBinding = gameRepo.IsMouseCaptured.Bind()
      .OnValue((isMouseCaptured) =>
      {
        if (isMouseCaptured)
        {
          Input(new Input.EnableInput());
          return;
        }

        Input(new Input.DisableInput());
      });

    _playerGlobalPositionBinding = gameRepo.PlayerGlobalPosition.Bind()
      .OnValue((playerGlobalPosition) => Input(new Input.TargetPositionChanged(playerGlobalPosition)));
  }

  public override void OnStop()
  {
    _isMouseCapturedBinding?.Dispose();
    _playerGlobalPositionBinding?.Dispose();
  }

  public override ILogicBlockSaveData GetSaveData(LogicBlockData data) =>
    new PlayerCameraLogicSaveData { Data = data };
}

[Meta, Id("player_camera_logic_save_data")]
public partial class PlayerCameraLogicSaveData : ILogicBlockSaveData
{
  [Save("data")]
  public required LogicBlockData Data { get; init; }
}
