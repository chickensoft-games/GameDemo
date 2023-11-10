namespace GameDemo.Tests;

using Chickensoft.GoDotTest;
using Godot;
using Moq;
using Shouldly;

public class PlayerCameraLogicStateInputEnabledTest : TestClass {
  private PlayerCameraLogic.IFakeContext _context = default!;
  private PlayerCameraSettings _settings = default!;
  private PlayerCameraLogic.Data _data = default!;
  private Mock<IAppRepo> _appRepo = default!;
  private Mock<IGameRepo> _gameRepo = default!;

  public PlayerCameraLogicStateInputEnabledTest(Node testScene) :
    base(testScene) { }

  [Setup]
  public void Setup() {
    _settings = new();
    _data = new();
    _context = PlayerCameraLogic.CreateFakeContext();

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
  public void GoesToInputDisabled() {
    var state = new PlayerCameraLogic.State.InputEnabled(_context);
    var nextState = state.On(new PlayerCameraLogic.Input.DisableInput());

    nextState.ShouldBeOfType<PlayerCameraLogic.State.InputDisabled>();
  }

  [Test]
  public void UpdatesTargetAnglesWhenMouseInputOccurs() {
    var state = new PlayerCameraLogic.State.InputEnabled(_context);

    var targetAngleHorizontal = _data.TargetAngleHorizontal;
    var targetAngleVertical = _data.TargetAngleVertical;

    var motion = new InputEventMouseMotion {
      Relative = Vector2.One
    };

    var nextState = state.On(
      new PlayerCameraLogic.Input.MouseInputOccurred(motion)
    );

    state.ShouldBeSameAs(nextState);

    _data.TargetAngleHorizontal.ShouldNotBe(targetAngleHorizontal);
    _data.TargetAngleVertical.ShouldNotBe(targetAngleVertical);
  }
}
