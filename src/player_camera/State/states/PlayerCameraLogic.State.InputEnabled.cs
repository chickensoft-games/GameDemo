namespace GameDemo;

using Godot;

public partial class PlayerCameraLogic {
  public partial record State {
    /// <summary>The state of the player camera.</summary>
    public record InputEnabled : State,
    IGet<Input.DisableInput>, IGet<Input.MouseInputOccurred> {
      public IState On(in Input.DisableInput input) => new InputDisabled();

      public IState On(in Input.MouseInputOccurred input) {
        var settings = Get<PlayerCameraSettings>();
        var data = Get<Data>();

        var targetAngleHorizontal = data.TargetAngleHorizontal +
          (-input.Motion.Relative.X * settings.MouseSensitivity);
        var targetAngleVertical = Mathf.Clamp(
          data.TargetAngleVertical +
          (-input.Motion.Relative.Y * settings.MouseSensitivity),
          settings.VerticalMin,
          settings.VerticalMax
        );

        data.TargetAngleHorizontal = targetAngleHorizontal;
        data.TargetAngleVertical = targetAngleVertical;
        return this;
      }
    }
  }
}
