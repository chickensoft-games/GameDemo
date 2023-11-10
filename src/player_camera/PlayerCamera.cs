namespace GameDemo;

using Chickensoft.AutoInject;
using Chickensoft.GodotNodeInterfaces;
using Chickensoft.PowerUps;
using Godot;
using SuperNodes.Types;

/// <summary>
/// Camera interface. This is how the camera node is exposed to its logic block,
/// allowing the logic block to read properties of the camera without having
/// to know about its implementation. This interface can easily be mocked,
/// allowing the camera logic block to be unit-tested.
/// </summary>
public interface IPlayerCamera : INode3D {
  /// <summary>Camera system's overall offset that doesn't change during
  /// runtime. The camera's position is determined by the target offset
  /// (usually the player's global position) added to this.</summary>
  Vector3 Offset { get; }

  /// <summary>The local position of the spring arm target node (it's
  /// the node that is a child of the spring arm so it moves wherever the
  /// spring arm says it should). We don't just directly make the camera a child
  /// of the spring arm because we actually want to smooth the spring arm
  /// changes via lerping. There's going to be some clipping, but we're okay
  /// with that for an improved feel.</summary>
  Vector3 SpringArmTargetPosition { get; }

  /// <summary>Camera's local position within the camera system.</summary>
  Vector3 CameraLocalPosition { get; }
  /// <summary>Horizontal gimbal rotation in euler angles.</summary>
  Vector3 GimbalRotationHorizontal { get; }
  /// <summary>Vertical gimbal rotation in euler angles.</summary>
  Vector3 GimbalRotationVertical { get; }

  /// <summary>Camera's global transform basis.</summary>
  Basis CameraBasis { get; }

  /// <summary>Local position of the offset node that is a parent of the camera.
  /// Changing this allows us to offset the camera side to side when strafing
  /// so that you can see  where you're going.
  /// </summary>
  Vector3 OffsetPosition { get; }

  /// <summary>Sets the current camera to the player camera.</summary>
  void UsePlayerCamera();
}

[SuperNode(typeof(Dependent), typeof(AutoNode))]
public partial class PlayerCamera : Node3D, IPlayerCamera {
  public override partial void _Notification(int what);

  #region Dependencies
  [Dependency]
  public IGameRepo GameRepo => DependOn<IGameRepo>();
  [Dependency]
  public IAppRepo AppRepo => DependOn<IAppRepo>();
  #endregion Dependencies

  #region State
  public IPlayerCameraLogic CameraLogic { get; set; } = default!;
  public PlayerCameraLogic.IBinding CameraBinding {
    get; set;
  } = default!;
  #endregion State

  #region Exports
  [Export]
  public Vector3 Offset { get; set; } = Vector3.Zero;

  [Export(PropertyHint.ResourceType, "PlayerCameraSettings")]
  public PlayerCameraSettings Settings { get; set; } = new();
  #endregion Exports

  #region Nodes
  [Node("%Offset")]
  public INode3D OffsetNode { get; set; } = default!;

  [Node("%GimbalHorizontal")]
  public INode3D GimbalHorizontalNode { get; set; } = default!;
  [Node("%GimbalVertical")]
  public INode3D GimbalVerticalNode { get; set; } = default!;
  [Node("%Camera3D")]
  public ICamera3D CameraNode { get; set; } = default!;
  [Node("%SpringArmTarget")]
  public INode3D SpringArmTarget { get; set; } = default!;
  #endregion Nodes

  #region Computed
  public Vector3 SpringArmTargetPosition => SpringArmTarget.Position;
  public Vector3 CameraLocalPosition => CameraNode.Position;
  public Vector3 GimbalRotationHorizontal => GimbalHorizontalNode.Rotation;
  public Vector3 GimbalRotationVertical => GimbalVerticalNode.Rotation;
  public Basis CameraBasis => GimbalHorizontalNode.GlobalTransform.Basis;
  // Camera offset for when strafing, etc, so you can see where you're going.
  public Vector3 OffsetPosition => OffsetNode.Position;
  #endregion Computed

  public void Setup() {
    CameraLogic = new PlayerCameraLogic(this, Settings, AppRepo, GameRepo);
    SetPhysicsProcess(true);
  }

  public void OnResolved() {
    CameraBinding = CameraLogic.Bind();
    CameraBinding
      .Handle<PlayerCameraLogic.Output.GimbalRotationChanged>(
        (output) => {
          GimbalHorizontalNode.Rotation = output.GimbalRotationHorizontal;
          GimbalVerticalNode.Rotation = output.GimbalRotationVertical;
        }
      ).Handle<PlayerCameraLogic.Output.GlobalTransformChanged>(
        (output) => GlobalTransform = output.GlobalTransform
      ).Handle<PlayerCameraLogic.Output.CameraLocalPositionChanged>(
        (output) => CameraNode.Position = output.CameraLocalPosition
      ).Handle<PlayerCameraLogic.Output.CameraOffsetChanged>(
        (output) => OffsetNode.Position = output.Offset
      );

    CameraLogic.Start();
  }

  public void OnPhysicsProcess(double delta)
    => CameraLogic.Input(
      new PlayerCameraLogic.Input.PhysicsTicked(delta)
    );

  public override void _Input(InputEvent @event) {
    if (@event is InputEventMouseMotion motion) {
      CameraLogic.Input(new PlayerCameraLogic.Input.MouseInputOccurred(motion));
    }
  }

  public void UsePlayerCamera() => CameraNode.MakeCurrent();

  public void OnExitTree() {
    CameraLogic.Stop();
    CameraBinding.Dispose();
  }
}
