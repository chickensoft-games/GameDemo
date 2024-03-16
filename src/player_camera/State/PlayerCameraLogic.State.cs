namespace GameDemo;

using Chickensoft.LogicBlocks;
using Godot;

public partial class PlayerCameraLogic {
  public interface IState : IStateLogic<IState> {
  }

  /// <summary>
  ///   Overall player camera state. This would be abstract, but it's helpful to
  ///   be able to instantiate it by itself for easier testing.
  /// </summary>
  public partial record State
    : StateLogic<IState>, IState,
      IGet<Input.PhysicsTicked>,
      IGet<Input.TargetPositionChanged>,
      IGet<Input.TargetOffsetChanged> {
    public State() {
      OnAttach(
        () => {
          var gameRepo = Get<IGameRepo>();
          gameRepo.IsMouseCaptured.Sync += OnMouseCaptured;
          gameRepo.PlayerGlobalPosition.Sync += OnPlayerGlobalPositionChanged;
        }
      );

      OnDetach(
        () => {
          var gameRepo = Get<IGameRepo>();
          gameRepo.IsMouseCaptured.Sync -= OnMouseCaptured;
          gameRepo.PlayerGlobalPosition.Sync -= OnPlayerGlobalPositionChanged;
        }
      );
    }

    internal void OnMouseCaptured(bool isMouseCaptured) {
      if (isMouseCaptured) {
        Input(new Input.EnableInput());
        return;
      }

      Input(new Input.DisableInput());
    }

    internal void OnPlayerGlobalPositionChanged(Vector3 position) =>
      Input(new Input.TargetPositionChanged(position));

    internal void OnCameraTargetOffsetChanged(Vector3 targetOffset) =>
      Input(new Input.TargetOffsetChanged(targetOffset));

    public IState On(in Input.PhysicsTicked input) {
      var camera = Get<IPlayerCamera>();
      var gameRepo = Get<IGameRepo>();
      var settings = Get<PlayerCameraSettings>();
      var data = Get<Data>();

      // Lerp to the desired horizontal angle.
      var rotationHorizontal = camera.GimbalRotationHorizontal;
      var rotationHorizontalY = Mathf.RadToDeg(rotationHorizontal.Y);
      rotationHorizontal.Y = Mathf.DegToRad(Mathf.Lerp(
        rotationHorizontalY,
        data.TargetAngleHorizontal,
        (float)input.Delta * settings.HorizontalRotationAcceleration
      ));

      // Lerp to the desired vertical angle.
      var rotationVertical = camera.GimbalRotationVertical;
      var rotationVerticalX = Mathf.RadToDeg(rotationVertical.X);
      rotationVertical.X = Mathf.DegToRad(Mathf.Lerp(
        rotationVerticalX,
        data.TargetAngleVertical,
        (float)input.Delta * settings.VerticalRotationAcceleration
      ));

      // This triggers the camera to update its gimbal nodes.
      // This keeps us from having to know about the camera's implementation
      // details.
      Output(new Output.GimbalRotationChanged(
        rotationHorizontal, rotationVertical
      ));

      // We can just read properties off the camera and set them on the game.
      gameRepo.SetCameraBasis(camera.CameraBasis);

      // Lerp to the desired target position.
      var transform = camera.GlobalTransform;
      transform.Origin = data.TargetPosition + camera.Offset;
      var globalTransform = camera.GlobalTransform.InterpolateWith(
        transform, (float)input.Delta * settings.FollowSpeed
      ).Orthonormalized();

      Output(new Output.GlobalTransformChanged(globalTransform));

      // Lerp camera inside system to spring arm target position
      var springArmTargetPosition = camera.SpringArmTargetPosition;
      var cameraLocalPosition = camera.CameraLocalPosition;
      var springArmTargetPositionLerp = cameraLocalPosition.Lerp(
        springArmTargetPosition, (float)input.Delta * settings.SpringArmAdjSpeed
      );

      Output(
        new Output.CameraLocalPositionChanged(springArmTargetPositionLerp)
      );

      // Lerp the camera local offset
      var offset = camera.OffsetPosition.Lerp(
        data.TargetOffset, (float)input.Delta * settings.OffsetAdjSpeed
      );

      Output(new Output.CameraOffsetChanged(offset));

      return this;
    }

    public IState On(in Input.TargetPositionChanged input) {
      var data = Get<Data>();
      data.TargetPosition = input.TargetPosition;
      return this;
    }

    public IState On(in Input.TargetOffsetChanged input) {
      var data = Get<Data>();
      data.TargetOffset = input.TargetOffset;
      return this;
    }
  }
}
