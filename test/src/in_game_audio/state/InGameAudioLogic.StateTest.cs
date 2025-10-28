namespace GameDemo.Tests;

using Chickensoft.GoDotTest;
using Chickensoft.LogicBlocks;
using Godot;
using Moq;
using Shouldly;

public class InGameAudioLogicStateTest : TestClass
{
  private IFakeContext _context = default!;
  private Mock<IAppRepo> _appRepo = default!;
  private Mock<IGameRepo> _gameRepo = default!;
  private InGameAudioLogic.State _state = default!;

  public InGameAudioLogicStateTest(Node testScene) : base(testScene) { }

  [Setup]
  public void Setup()
  {
    _appRepo = new();
    _gameRepo = new();

    _state = new();
    _context = _state.CreateFakeContext();
    _context.Set(_appRepo.Object);
    _context.Set(_gameRepo.Object);
  }

  [Test]
  public void Subscribes()
  {
    _state.Attach(_context);

    _gameRepo.VerifyAdd(
      repo => repo.CoinCollectionStarted += _state.OnCoinCollectionStarted
    );
    _gameRepo.VerifyAdd(
      repo => repo.JumpshroomUsed += _state.OnJumpshroomUsed
    );
    _gameRepo.VerifyAdd(
      repo => repo.Ended += _state.OnGameEnded
    );
    _gameRepo.VerifyAdd(
      repo => repo.Jumped += _state.OnJumped
    );
    _appRepo.VerifyAdd(
      repo => repo.MainMenuEntered += _state.OnMainMenuEntered
    );
    _appRepo.VerifyAdd(
      repo => repo.GameEntered += _state.OnGameEntered
    );

    _state.Detach();

    _gameRepo.VerifyRemove(
      repo => repo.CoinCollectionStarted -= _state.OnCoinCollectionStarted
    );
    _gameRepo.VerifyRemove(
      repo => repo.JumpshroomUsed -= _state.OnJumpshroomUsed
    );
    _gameRepo.VerifyRemove(
      repo => repo.Ended -= _state.OnGameEnded
    );
    _gameRepo.VerifyRemove(
      repo => repo.Jumped -= _state.OnJumped
    );
    _appRepo.VerifyRemove(
      repo => repo.MainMenuEntered -= _state.OnMainMenuEntered
    );
    _appRepo.VerifyRemove(
      repo => repo.GameEntered -= _state.OnGameEntered
    );
  }

  [Test]
  public void OnCoinCollectionStarted()
  {
    _state.OnCoinCollectionStarted(new Mock<ICoin>().Object);

    _context.Outputs.ShouldBe([
      new InGameAudioLogic.Output.PlayCoinCollected()
    ]);
  }

  [Test]
  public void OnJumpshroomUsed()
  {
    _state.OnJumpshroomUsed();

    _context.Outputs.ShouldBe([
      new InGameAudioLogic.Output.PlayBounce()
    ]);
  }

  [Test]
  public void OnGameEndedLost()
  {
    _state.OnGameEnded(GameOverReason.Lost);

    _context.Outputs.ShouldBe([
      new InGameAudioLogic.Output.StopGameMusic(),
      new InGameAudioLogic.Output.PlayPlayerDied()
    ]);
  }

  [Test]
  public void OnGameEndedOther()
  {
    _state.OnGameEnded(GameOverReason.Quit);

    _context.Outputs.ShouldBe([
      new InGameAudioLogic.Output.StopGameMusic()
    ]);
  }

  [Test]
  public void OnJumped()
  {
    _state.OnJumped();

    _context.Outputs.ShouldBe([
      new InGameAudioLogic.Output.PlayJump()
    ]);
  }

  [Test]
  public void OnMainMenuEntered()
  {
    _state.OnMainMenuEntered();

    _context.Outputs.ShouldBe([
      new InGameAudioLogic.Output.PlayMainMenuMusic()
    ]);
  }

  [Test]
  public void OnGameEntered()
  {
    _state.OnGameEntered();

    _context.Outputs.ShouldBe([
      new InGameAudioLogic.Output.PlayGameMusic()
    ]);
  }
}
