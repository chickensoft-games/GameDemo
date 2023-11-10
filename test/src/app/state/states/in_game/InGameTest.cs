namespace GameDemo.Tests;

using System;
using Chickensoft.GoDotTest;
using Godot;
using Moq;
using Shouldly;

public class InGameTest : TestClass {
  private AppLogic.IFakeContext _context = default!;
  private Mock<IAppRepo> _appRepo = default!;
  private AppLogic.State.InGame _state = default!;

  public InGameTest(Node testScene) : base(testScene) { }

  [Setup]
  public void Setup() {
    _context = AppLogic.CreateFakeContext();
    _appRepo = new();

    _context.Set(_appRepo.Object);
    _state = new(_context);
  }

  [Test]
  public void EntersAndExits() {
    var parent = new AppLogic.State(_context);

    _appRepo.Setup(repo => repo.OnStartGame());

    _state.Enter(parent);

    _context.Outputs.ShouldBe(
      new object[] { new AppLogic.Output.ShowGame() }
    );
    _appRepo.VerifyAdd(repo => repo.GameEnded += _state.OnGameOver);

    _context.Reset();

    _state.Exit(parent);

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
