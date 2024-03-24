namespace GameDemo;

using Godot;

public partial class PlayerCameraLogic {
  public partial record State {
    /// <summary>The state of the player camera.</summary>
    public record InputEnabled : State,
    IGet<Input.DisableInput>, IGet<Input.MouseInputOccurred>
      , IGet<Input.JoyPadInputOccurred> {
      public IState On(Input.DisableInput input) => new InputDisabled();

      public IState On(Input.MouseInputOccurred input) {
        var settings = Context.Get<PlayerCameraSettings>();
        var data = Context.Get<Data>();

        var targetAngleVertical = Mathf.Clamp(
          data.TargetAngleVertical +
          (-input.Motion.Relative.Y * settings.MouseSensitivity),
          settings.VerticalMin,
          settings.VerticalMax
        );

        data.TargetAngleHorizontal += -input.Motion.Relative.X * settings.MouseSensitivity;
        data.TargetAngleVertical = targetAngleVertical;

        return this;
      }

      public IState On(Input.JoyPadInputOccurred input) {
        var settings = Context.Get<PlayerCameraSettings>();
        var data = Context.Get<Data>();

        if (input.Motion.Axis == JoyAxis.RightX) {
          data.TargetAngleHorizontal += -input.Motion.AxisValue * settings.JoypadSensitivity;
        }

        if (input.Motion.Axis == JoyAxis.RightY) {
          var targetAngleVertical = Mathf.Clamp(
             data.TargetAngleVertical +
             (-input.Motion.AxisValue * settings.JoypadSensitivity),
             settings.VerticalMin,
             settings.VerticalMax
           );
          data.TargetAngleVertical = targetAngleVertical;
        }
        return this;
      }
    }
  }
}
