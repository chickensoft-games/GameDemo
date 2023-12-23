namespace GameDemo.Tests;

using System;
using Chickensoft.GoDotCollections;
using Chickensoft.GoDotTest;
using Chickensoft.LogicBlocks;
using Godot;
using Moq;
using Shouldly;

public class InGameTest : TestClass {
  private IFakeContext _context = default!;
  private Mock<IAppRepo> _appRepo = default!;
  private AppLogic.State.InGame _state = default!;
  private AutoProp<bool> _isMouseCaptured = default!;

  public InGameTest(Node testScene) : base(testScene) { }

  [Setup]
  public void Setup() {
    _appRepo = new();
    _isMouseCaptured = new(false);
    _appRepo.Setup(repo => repo.IsMouseCaptured)
      .Returns(_isMouseCaptured);

    _state = new();
    _context = _state.CreateFakeContext();
    _context.Set(_appRepo.Object);
  }

  [Test]
  public void EntersAndExits() {
    _appRepo.Reset();

    _appRepo.Setup(repo => repo.OnStartGame());

    var parent = new AppLogic.State();
    _state.Enter(parent);

    _appRepo.VerifyAll();

    _context.Outputs.ShouldBe(
      new object[] { new AppLogic.Output.ShowGame() }
    );
  }

  [Test]
  public void Subscribes() {
    _state.Attach(_context);
    _appRepo.VerifyAdd(repo => repo.GameEnded += _state.OnGameOver);

    _state.Detach();
    _appRepo.VerifyRemove(repo => repo.GameEnded -= _state.OnGameOver);
  }

  [Test]
  public void WatchesForGameOver() {
    _state.OnGameOver(GameOverReason.PlayerDied);

    _context.Inputs.ShouldBe(
      new object[] { new AppLogic.Input.GameOver(GameOverReason.PlayerDied) }
    );
  }

  [Test]
  public void GoesToAppropriateStateOnGameOver() {
    _appRepo.Setup(repo => repo.Pause());

    _state.On(new AppLogic.Input.GameOver(GameOverReason.PlayerWon))
      .ShouldBeOfType<AppLogic.State.WonGame>();

    _state.On(new AppLogic.Input.GameOver(GameOverReason.PlayerDied))
      .ShouldBeOfType<AppLogic.State.LostGame>();

    _state.On(new AppLogic.Input.GameOver(GameOverReason.Exited))
      .ShouldBeOfType<AppLogic.State.MainMenu>();

    Should.Throw<InvalidOperationException>(
      () => _state.On(new AppLogic.Input.GameOver((GameOverReason)13))
    );

    _appRepo.Verify(repo => repo.Pause());
  }

  [Test]
  public void LeavesGame() {
    _state.On(new AppLogic.Input.GoToMainMenu())
      .ShouldBeOfType<AppLogic.State.LeavingGame>();
  }
}
