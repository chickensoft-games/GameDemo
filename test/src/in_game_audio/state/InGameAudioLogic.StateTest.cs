namespace GameDemo.Tests;

using Chickensoft.GoDotTest;
using Godot;
using Moq;
using Shouldly;

public class InGameAudioLogicStateTest : TestClass
{
  private StateTester _context = default!;
  private Mock<IAppRepo> _appRepo = default!;
  private Mock<IGameRepo> _gameRepo = default!;
  private InGameAudioLogicState _state = default!;

  public InGameAudioLogicStateTest(Node testScene) : base(testScene) { }

  [Setup]
  public void Setup()
  {
    _appRepo = new ();
    _gameRepo = new ();

    _state = new();
    _context = _state.Test();
    _context.Set(_appRepo.Object);
    _context.Set(_gameRepo.Object);
  }

  [Test]
  public void OnCoinCollectionStarted()
  {
    _state.OnCoinCollected();

    _context.Outputs.ShouldBe([
      new InGameAudioLogicState.Output.PlayCoinCollected()
    ]);
  }

  [Test]
  public void OnJumpshroomUsed()
  {
    _state.OnJumpshroomUsed();

    _context.Outputs.ShouldBe([
      new InGameAudioLogicState.Output.PlayBounce()
    ]);
  }

  [Test]
  public void OnGameEndedLost()
  {
    _state.OnGameEnded(GameOverReason.Lost);

    _context.Outputs.ShouldBe([
      new InGameAudioLogicState.Output.StopGameMusic(),
      new InGameAudioLogicState.Output.PlayPlayerDied()
    ]);
  }

  [Test]
  public void OnGameEndedOther()
  {
    _state.OnGameEnded(GameOverReason.Won);

    _context.Outputs.ShouldBe([
      new InGameAudioLogicState.Output.StopGameMusic()
    ]);
  }

  [Test]
  public void OnJumped()
  {
    _state.OnJumped();

    _context.Outputs.ShouldBe([
      new InGameAudioLogicState.Output.PlayJump()
    ]);
  }

  [Test]
  public void OnMainMenuEntered()
  {
    _state.OnMainMenuEntered();

    _context.Outputs.ShouldBe([
      new InGameAudioLogicState.Output.PlayMainMenuMusic()
    ]);
  }

  [Test]
  public void OnGameEntered()
  {
    _state.OnGameEntered();

    _context.Outputs.ShouldBe([
      new InGameAudioLogicState.Output.PlayGameMusic()
    ]);
  }
}
