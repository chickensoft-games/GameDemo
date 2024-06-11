namespace GameDemo;

using Chickensoft.AutoInject;
using Chickensoft.GodotNodeInterfaces;
using Chickensoft.SaveFileBuilder;
using Godot;
using Chickensoft.Introspection;

/// <summary>
///   Camera interface. This is how the camera node is exposed to its logic block,
///   allowing the logic block to read properties of the camera without having
///   to know about its implementation. This interface can easily be mocked,
///   allowing the camera logic block to be unit-tested.
/// </summary>
public interface IPlayerCamera : INode3D {
  IPlayerCameraLogic CameraLogic { get; }

  /// <summary>
  ///   Camera system's overall offset that doesn't change during
  ///   runtime. The camera's position is determined by the target offset
  ///   (usually the player's global position) added to this.
  /// </summary>
  Vector3 Offset { get; }

  /// <summary>
  ///   The local position of the spring arm target node (it's
  ///   the node that is a child of the spring arm so it moves wherever the
  ///   spring arm says it should). We don't just directly make the camera a child
  ///   of the spring arm because we actually want to smooth the spring arm
  ///   changes via lerping. There's going to be some clipping, but we're okay
  ///   with that for an improved feel.
  /// </summary>
  Vector3 SpringArmTargetPosition { get; }

  /// <summary>Camera's local position within the camera system.</summary>
  Vector3 CameraLocalPosition { get; }

  /// <summary>Horizontal gimbal rotation in euler angles.</summary>
  Vector3 GimbalRotationHorizontal { get; }

  /// <summary>Vertical gimbal rotation in euler angles.</summary>
  Vector3 GimbalRotationVertical { get; }

  /// <summary>Camera's global transform basis.</summary>
  Basis CameraBasis { get; }

  /// <summary>
  ///   Local position of the offset node that is a parent of the camera.
  ///   Changing this allows us to offset the camera side to side when strafing
  ///   so that you can see  where you're going.
  /// </summary>
  Vector3 OffsetPosition { get; }

  /// <summary>Sets the current camera to the player camera.</summary>
  void UsePlayerCamera();
}

[Meta(typeof(IAutoNode))]
public partial class PlayerCamera : Node3D, IPlayerCamera {
  public override void _Notification(int what) => this.Notify(what);

  #region Save

  [Dependency]
  public ISaveChunk<GameData> GameChunk => this.DependOn<ISaveChunk<GameData>>();
  public ISaveChunk<PlayerCameraData> PlayerCameraChunk { get; set; } = default!;
  #endregion Save

  #region State
  [Dependency] public IGameRepo GameRepo => this.DependOn<IGameRepo>();

  public IPlayerCameraLogic CameraLogic { get; set; } = default!;

  public PlayerCameraLogic.IBinding CameraBinding { get; set; } = default!;

  #endregion State

  #region Exports

  [Export] public Vector3 Offset { get; set; } = Vector3.Zero;

  [Export(PropertyHint.ResourceType, "PlayerCameraSettings")]
  public PlayerCameraSettings Settings { get; set; } = new();

  #endregion Exports

  #region Nodes

  [Node("%Offset")] public INode3D OffsetNode { get; set; } = default!;

  [Node("%GimbalHorizontal")]
  public INode3D GimbalHorizontalNode { get; set; } = default!;

  [Node("%GimbalVertical")]
  public INode3D GimbalVerticalNode { get; set; } = default!;

  [Node("%Camera3D")] public ICamera3D CameraNode { get; set; } = default!;

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
    CameraLogic = new PlayerCameraLogic();

    CameraLogic.Set(this as IPlayerCamera);
    CameraLogic.Set(Settings);
    CameraLogic.Set(GameRepo);

    CameraLogic.Save(
      () => new PlayerCameraLogic.Data {
        TargetPosition = Vector3.Zero,
        TargetAngleHorizontal = 0f,
        TargetAngleVertical = 0f,
        TargetOffset = Vector3.Zero
      }
    );

    PlayerCameraChunk = new SaveChunk<PlayerCameraData>(
      onSave: (chunk) => new PlayerCameraData() {
        StateMachine = (PlayerCameraLogic)CameraLogic,
        GlobalTransform = GlobalTransform,
        LocalPosition = CameraNode.Position,
        OffsetPosition = OffsetNode.Position,
      },
      onLoad: (chunk, data) => {
        CameraLogic.RestoreFrom(data.StateMachine);
        GlobalTransform = data.GlobalTransform;
        CameraNode.Position = data.LocalPosition;
        OffsetNode.Position = data.OffsetPosition;

        CameraLogic.Input(new PlayerCameraLogic.Input.PhysicsTicked(0d));
      }
    );

    SetPhysicsProcess(true);
  }

  public void OnResolved() {
    GameChunk.AddChunk(PlayerCameraChunk);

    CameraBinding = CameraLogic.Bind();
    CameraBinding
      .Handle((in PlayerCameraLogic.Output.GimbalRotationChanged output) => {
        GimbalHorizontalNode.Rotation = output.GimbalRotationHorizontal;
        GimbalVerticalNode.Rotation = output.GimbalRotationVertical;
      })
      .Handle((in PlayerCameraLogic.Output.GlobalTransformChanged output) =>
        GlobalTransform = output.GlobalTransform
      )
      .Handle(
        (in PlayerCameraLogic.Output.CameraLocalPositionChanged output) =>
          CameraNode.Position = output.CameraLocalPosition
      )
      .Handle((in PlayerCameraLogic.Output.CameraOffsetChanged output) =>
        OffsetNode.Position = output.Offset
      );

    CameraLogic.Start();
  }

  public void OnPhysicsProcess(double delta) {
    var xMotion = InputUtilities.GetJoyPadActionPressedMotion(
      "camera_left", "camera_right", JoyAxis.RightX
    );

    if (xMotion is not null) {
      CameraLogic.Input(new PlayerCameraLogic.Input.JoyPadInputOccurred(xMotion));
    }

    var yMotion = InputUtilities.GetJoyPadActionPressedMotion(
      "camera_up", "camera_down", JoyAxis.RightY
    );

    if (yMotion is not null) {
      CameraLogic.Input(new PlayerCameraLogic.Input.JoyPadInputOccurred(yMotion));
    }

    CameraLogic.Input(
      new PlayerCameraLogic.Input.PhysicsTicked(delta)
    );
  }

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
