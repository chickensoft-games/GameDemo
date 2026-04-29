namespace GameDemo.Tests;

using Chickensoft.GoDotTest;
using Godot;
using Moq;
using Shouldly;

public class InGameAudioLogicStateTest : TestClass
{
  private StateTester _context = default!;
  private IAppRepo _appRepo = default!;
  private IGameRepo _gameRepo = default!;
  private InGameAudioLogic.BaseState _state = default!;

  public InGameAudioLogicStateTest(Node testScene) : base(testScene) { }

  [Setup]
  public void Setup()
  {
    _appRepo = new AppRepo();
    _gameRepo = new GameRepo();
    _state = new();
    _context = _state.Test();
    _context.Set(_appRepo);
    _context.Set(_gameRepo);
    InGameAudioLogic.SetupSubscriptions(_appRepo, () => _state);
    InGameAudioLogic.SetupSubscriptions(_gameRepo, () => _state);
  }

  [Cleanup]
  public void Cleanup()
  {
    _appRepo.Dispose();
    _gameRepo.Dispose();
  }

  [Test]
  public void OnCoinCollectionStarted()
  {
    _gameRepo.StartCoinCollection(new Mock<ICoin>().Object);

    _context.Outputs.ShouldBe([
      new InGameAudioLogic.Output.PlayCoinCollected()
    ]);
  }

  [Test]
  public void OnJumpshroomUsed()
  {
    _gameRepo.OnJumpshroomUsed();

    _context.Outputs.ShouldBe([
      new InGameAudioLogic.Output.PlayBounce()
    ]);
  }

  [Test]
  public void OnGameEndedLost()
  {
    _gameRepo.OnGameEnded(GameOverReason.Lost);

    _context.Outputs.ShouldBe([
      new InGameAudioLogic.Output.StopGameMusic(),
      new InGameAudioLogic.Output.PlayPlayerDied()
    ]);
  }

  [Test]
  public void OnGameEndedOther()
  {
    _gameRepo.OnGameEnded(GameOverReason.Won);

    _context.Outputs.ShouldBe([
      new InGameAudioLogic.Output.StopGameMusic()
    ]);
  }

  [Test]
  public void OnJumped()
  {
    _gameRepo.OnJump();

    _context.Outputs.ShouldBe([
      new InGameAudioLogic.Output.PlayJump()
    ]);
  }

  [Test]
  public void OnMainMenuEntered()
  {
    _appRepo.OnMainMenuEntered();

    _context.Outputs.ShouldBe([
      new InGameAudioLogic.Output.PlayMainMenuMusic()
    ]);
  }

  [Test]
  public void OnGameEntered()
  {
    _appRepo.OnEnterGame();

    _context.Outputs.ShouldBe([
      new InGameAudioLogic.Output.PlayGameMusic()
    ]);
  }
}
