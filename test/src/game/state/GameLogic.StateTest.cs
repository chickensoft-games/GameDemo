namespace GameDemo.Tests;

using Chickensoft.GoDotTest;
using Chickensoft.LogicBlocks;
using Godot;
using Moq;
using Shouldly;

public class GameLogicStateTest : TestClass {
  private IFakeContext _context = default!;
  private Mock<IAppRepo> _appRepo = default!;
  private GameLogic.State _state = default!;

  public GameLogicStateTest(Node testScene) : base(testScene) { }

  [Setup]
  public void Setup() {
    _appRepo = new();

    _state = new();
    _context = _state.CreateFakeContext();
    _context.Set(_appRepo.Object);
  }

  [Test]
  public void Subscribes() {
    _state.Attach(_context);

    _appRepo.VerifyAdd(
      (repo) => repo.GameStarting += _state.GameAboutToStart
    );
    _appRepo.VerifyAdd(
      (repo) => repo.GamePaused += _state.GamePaused
    );
    _appRepo.VerifyAdd(
      (repo) => repo.GameResumed += _state.GameResumed
    );

    _state.Detach();

    _appRepo.VerifyRemove(
      (repo) => repo.GameStarting -= _state.GameAboutToStart
    );
    _appRepo.VerifyRemove(
      (repo) => repo.GamePaused -= _state.GamePaused
    );
    _appRepo.VerifyRemove(
      (repo) => repo.GameResumed -= _state.GameResumed
    );
  }

  [Test]
  public void ChangesToThirdPersonCameraWhenGameIsAboutToStart() {
    _state.GameAboutToStart();

    _context.Outputs.ShouldBe(new object[] {
      new GameLogic.Output.ChangeToThirdPersonCamera()
    });
  }

  [Test]
  public void UpdatesPauseModeOnGamePaused() {
    _state.GamePaused();

    _context.Outputs.ShouldBe(new object[] {
      new GameLogic.Output.SetPauseMode(true)
    });
  }

  [Test]
  public void UpdatesPauseModeOnGameResumed() {
    _state.GameResumed();

    _context.Outputs.ShouldBe(new object[] {
      new GameLogic.Output.SetPauseMode(false)
    });
  }

  [Test]
  public void SetsNumCoinsAtStartOnInitialize() {
    var numCoins = 3;

    _appRepo.Setup((repo) => repo.OnNumCoinsAtStart(numCoins));
    _state.On(new GameLogic.Input.Initialize(numCoins));

    _appRepo.VerifyAll();
  }
}
