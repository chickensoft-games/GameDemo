namespace GameDemo.Tests;

using System.Diagnostics.CodeAnalysis;
using Chickensoft.GoDotTest;
using Chickensoft.LogicBlocks;
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
public class PlayerCameraLogicStateInputEnabledTest : TestClass
{
  private IFakeContext _context = default!;
  private PlayerCameraSettings _settings = default!;
  private PlayerCameraLogic.Data _data = default!;
  private Mock<IAppRepo> _appRepo = default!;
  private Mock<IGameRepo> _gameRepo = default!;
  private PlayerCameraLogic.State.InputEnabled _state = default!;

  public PlayerCameraLogicStateInputEnabledTest(Node testScene) :
    base(testScene)
  { }

  [Setup]
  public void Setup()
  {
    _state = new();
    _context = _state.CreateFakeContext();
    _settings = new();
    _data = new()
    {
      TargetPosition = Vector3.Zero,
      TargetAngleHorizontal = 0,
      TargetAngleVertical = 0,
      TargetOffset = Vector3.Zero
    };

    _appRepo = new();
    _gameRepo = new();

    // Automatically mock the logic block context to provide mock versions
    // of everything the state needs.
    _context.Set(_settings);
    _context.Set(_data);
    _context.Set(_appRepo.Object);
    _context.Set(_gameRepo.Object);
  }

  [CleanupAll]
  public void CleanupAll() => _settings.Dispose();

  [Test]
  public void GoesToInputDisabled()
  {
    var next = _state.On(new PlayerCameraLogic.Input.DisableInput());

    next.State.ShouldBeOfType<PlayerCameraLogic.State.InputDisabled>();
  }

  [Test]
  public void UpdatesTargetAnglesWhenMouseInputOccurs()
  {
    var targetAngleHorizontal = _data.TargetAngleHorizontal;
    var targetAngleVertical = _data.TargetAngleVertical;

    var motion = new InputEventMouseMotion
    {
      Relative = Vector2.One
    };

    var next = _state.On(
      new PlayerCameraLogic.Input.MouseInputOccurred(motion)
    );

    _state.ShouldBeSameAs(next.State);

    _data.TargetAngleHorizontal.ShouldNotBe(targetAngleHorizontal);
    _data.TargetAngleVertical.ShouldNotBe(targetAngleVertical);
  }

  [Test]
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
      new PlayerCameraLogic.Input.JoyPadInputOccurred(motion)
    );

    _state.ShouldBeSameAs(next.State);

    var motion2 = new InputEventJoypadMotion
    {
      Axis = JoyAxis.RightY,
      AxisValue = 3,
      Device = 0
    };

    var next2 = _state.On(
      new PlayerCameraLogic.Input.JoyPadInputOccurred(motion2)
    );

    _state.ShouldBeSameAs(next2.State);

    _data.TargetAngleHorizontal.ShouldNotBe(targetAngleHorizontal);
    _data.TargetAngleVertical.ShouldNotBe(targetAngleVertical);
  }
}
