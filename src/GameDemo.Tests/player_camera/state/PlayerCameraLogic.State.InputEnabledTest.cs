namespace GameDemo.Tests;

using System.Diagnostics.CodeAnalysis;
using Godot;
using Moq;
using Shouldly;

[
  SuppressMessage(
    "Design",
    "CA1001",
    Justification = "Disposable field is disposed in cleanup"
  )
]
[Collection(Constants.HEADLESS)]
public class PlayerCameraLogicStateInputEnabledTest : IDisposable
{
  private readonly StateTester _context;
  private readonly PlayerCameraSettings _settings = new();
  private readonly PlayerCameraLogic.Data _data = new()
  {
    TargetPosition = Vector3.Zero,
    TargetAngleHorizontal = 0,
    TargetAngleVertical = 0,
    TargetOffset = Vector3.Zero
  };
  private readonly Mock<IAppRepo> _appRepo = new();
  private readonly Mock<IGameRepo> _gameRepo = new();
  private readonly PlayerCameraLogicState.InputEnabled _state = new();

  public PlayerCameraLogicStateInputEnabledTest()
  {
    _context = _state.Test();

    // Automatically mock the logic block context to provide mock versions
    // of everything the state needs.
    _context.Set(_settings);
    _context.Set(_data);
    _context.Set(_appRepo.Object);
    _context.Set(_gameRepo.Object);
  }

  public void Dispose()
  {
    _settings.Dispose();
    GC.SuppressFinalize(this);
  }

  [Fact]
  public void GoesToInputDisabled()
  {
    var next = _state.On(new PlayerCameraLogicState.Input.DisableInput());

    next.IsAssignableTo(typeof(PlayerCameraLogicState.InputDisabled)).ShouldBeTrue();
  }

  [Fact]
  public void UpdatesTargetAnglesWhenMouseInputOccurs()
  {
    var targetAngleHorizontal = _data.TargetAngleHorizontal;
    var targetAngleVertical = _data.TargetAngleVertical;

    var motion = new InputEventMouseMotion
    {
      Relative = Vector2.One
    };

    var next = _state.On(
      new PlayerCameraLogicState.Input.MouseInputOccurred(motion)
    );

    _state.ShouldBeOfType(next);

    _data.TargetAngleHorizontal.ShouldNotBe(targetAngleHorizontal);
    _data.TargetAngleVertical.ShouldNotBe(targetAngleVertical);
  }

  [Fact]
  public void UpdatesTargetAnglesWhenJoypadInputOccurs()
  {
    var targetAngleHorizontal = _data.TargetAngleHorizontal;
    var targetAngleVertical = _data.TargetAngleVertical;

    var motion = new InputEventJoypadMotion
    {
      Axis = JoyAxis.RightX,
      AxisValue = 3,
      Device = 0
    };

    var next = _state.On(
      new PlayerCameraLogicState.Input.JoyPadInputOccurred(motion)
    );

    _state.ShouldBeOfType(next);

    var motion2 = new InputEventJoypadMotion
    {
      Axis = JoyAxis.RightY,
      AxisValue = 3,
      Device = 0
    };

    var next2 = _state.On(
      new PlayerCameraLogicState.Input.JoyPadInputOccurred(motion2)
    );

    _state.ShouldBeOfType(next2);

    _data.TargetAngleHorizontal.ShouldNotBe(targetAngleHorizontal);
    _data.TargetAngleVertical.ShouldNotBe(targetAngleVertical);
  }
}
