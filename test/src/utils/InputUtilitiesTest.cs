namespace GameDemo.Tests;

using Chickensoft.GoDotTest;
using Chickensoft.LogicBlocks;
using Godot;
using Moq;
using Shouldly;

public class InputUtilitiesTest : TestClass {

  private IFakeContext _context = default!;
  private float _deadZoneX = default!;
  private float _deadZoneY = default!;
  private PlayerCameraSettings _settings = default!;
  private Mock<IGameRepo> _gameRepo = default!;
  private PlayerCameraLogic.State.InputEnabled _state = default!;

  public InputUtilitiesTest(Node testScene) :
  base(testScene) { }

  [Setup]
  public void Setup() {
    _state = new();
    _context = _state.CreateFakeContext();
    _settings = new();
    _gameRepo = new();

    // Automatically mock the logic block context to provide mock versions
    // of everything the state needs.
    _context.Set(_settings);
    _context.Set(_gameRepo.Object);

    _deadZoneX = InputMap.ActionGetDeadzone("camera_right");
    _deadZoneY = InputMap.ActionGetDeadzone("camera_up");
  }

  [CleanupAll]
  public void CleanupAll() => _settings.Dispose();

  [Test]
  public void TriggerJoyPadWhenAxisIsPressed() {
    //Strength by default is 1!
    Input.ActionPress("camera_right");
    Input.ActionPress("camera_up");

    var xMotion = InputUtilities.GetJoyPadActionPressedMotion(
      "camera_left", "camera_right", JoyAxis.RightX
    );

    var yMotion = InputUtilities.GetJoyPadActionPressedMotion(
      "camera_up", "camera_down", JoyAxis.RightY
    );

    xMotion!.AxisValue.ShouldBe(1);
    yMotion!.AxisValue.ShouldBe(-1);
  }

  [Test]
  public void NotTriggerJoyPadWhenAxisIsPressedInDeadZone() {
    Input.ActionPress("camera_right", _deadZoneX - 0.05f);
    Input.ActionPress("camera_up", _deadZoneY - 0.05f);

    var xMotion = InputUtilities.GetJoyPadActionPressedMotion(
      "camera_left", "camera_right", JoyAxis.RightX
    );

    var yMotion = InputUtilities.GetJoyPadActionPressedMotion(
      "camera_up", "camera_down", JoyAxis.RightY
    );

    xMotion.ShouldBeNull();
    yMotion.ShouldBeNull();
  }
}
