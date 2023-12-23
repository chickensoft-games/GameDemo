namespace GameDemo.Tests;

using Chickensoft.GoDotTest;
using Chickensoft.LogicBlocks;
using Godot;
using Moq;
using Shouldly;

public class InGameAudioLogicStateTest : TestClass {
  private IFakeContext _context = default!;
  private Mock<IAppRepo> _appRepo = default!;
  private InGameAudioLogic.State _state = default!;

  public InGameAudioLogicStateTest(Node testScene) : base(testScene) { }

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
      (repo) => repo.CoinCollected += _state.OnCoinCollected
    );
    _appRepo.VerifyAdd(
      (repo) => repo.JumpshroomUsed += _state.OnJumpshroomUsed
    );
    _appRepo.VerifyAdd(
      (repo) => repo.GameEnded += _state.OnGameEnded
    );
    _appRepo.VerifyAdd(
      (repo) => repo.Jumped += _state.OnJumped
    );
    _appRepo.VerifyAdd(
      (repo) => repo.MainMenuEntered += _state.OnMainMenuEntered
    );
    _appRepo.VerifyAdd(
      (repo) => repo.GameStarting += _state.OnGameStarting
    );

    _state.Detach();

    _appRepo.VerifyRemove(
      (repo) => repo.CoinCollected -= _state.OnCoinCollected
    );
    _appRepo.VerifyRemove(
      (repo) => repo.JumpshroomUsed -= _state.OnJumpshroomUsed
    );
    _appRepo.VerifyRemove(
      (repo) => repo.GameEnded -= _state.OnGameEnded
    );
    _appRepo.VerifyRemove(
      (repo) => repo.Jumped -= _state.OnJumped
    );
    _appRepo.VerifyRemove(
      (repo) => repo.MainMenuEntered -= _state.OnMainMenuEntered
    );
    _appRepo.VerifyRemove(
      (repo) => repo.GameStarting -= _state.OnGameStarting
    );
  }

  [Test]
  public void OnCoinCollected() {
    _state.OnCoinCollected();

    _context.Outputs.ShouldBe(new object[] {
      new InGameAudioLogic.Output.PlayCoinCollected()
    });
  }

  [Test]
  public void OnJumpshroomUsed() {
    _state.OnJumpshroomUsed();

    _context.Outputs.ShouldBe(new object[] {
      new InGameAudioLogic.Output.PlayBounce()
    });
  }

  [Test]
  public void OnGameEnded() {
    _state.OnGameEnded(GameOverReason.PlayerDied);

    _context.Outputs.ShouldBe(new object[] {
      new InGameAudioLogic.Output.PlayPlayerDied()
    });
  }

  [Test]
  public void OnJumped() {
    _state.OnJumped();

    _context.Outputs.ShouldBe(new object[] {
      new InGameAudioLogic.Output.PlayJump()
    });
  }

  [Test]
  public void OnMainMenuEntered() {
    _state.OnMainMenuEntered();

    _context.Outputs.ShouldBe(new object[] {
      new InGameAudioLogic.Output.PlayMainMenuMusic()
    });
  }

  [Test]
  public void OnGameStarting() {
    _state.OnGameStarting();

    _context.Outputs.ShouldBe(new object[] {
      new InGameAudioLogic.Output.PlayGameMusic()
    });
  }
}
