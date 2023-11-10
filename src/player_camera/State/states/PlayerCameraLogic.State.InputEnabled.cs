namespace GameDemo;

using Godot;

public partial class PlayerCameraLogic {
  public partial record State {
    /// <summary>The state of the player camera.</summary>
    /// <param name="Context">Logic block context.</param>
    public record InputEnabled(IContext Context) : State(Context),
      IGet<Input.DisableInput>, IGet<Input.MouseInputOccurred> {
      public IState On(Input.DisableInput input) => new InputDisabled(Context);

      public IState On(Input.MouseInputOccurred input) {
        var settings = Context.Get<PlayerCameraSettings>();
        var data = Context.Get<Data>();

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
