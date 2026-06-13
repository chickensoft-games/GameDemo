namespace GameDemo.Tests;

using Moq;
using Shouldly;

public class InGameAudioLogicStateTest
{
  private readonly StateTester _context;
  private readonly Mock<IAppRepo> _appRepo = new ();
  private readonly Mock<IGameRepo> _gameRepo = new ();
  private readonly InGameAudioLogicState _state = new();

  public InGameAudioLogicStateTest()
  {
    _context = _state.Test();
    _context.Set(_appRepo.Object);
    _context.Set(_gameRepo.Object);
  }

  [Fact]
  public void OnCoinCollectionStarted()
  {
    _state.OnCoinCollected();

    _context.Outputs.ShouldBe([
      new InGameAudioLogicState.Output.PlayCoinCollected()
    ]);
  }

  [Fact]
  public void OnJumpshroomUsed()
  {
    _state.OnJumpshroomUsed();

    _context.Outputs.ShouldBe([
      new InGameAudioLogicState.Output.PlayBounce()
    ]);
  }

  [Fact]
  public void OnGameEndedLost()
  {
    _state.OnGameEnded(GameOverReason.Lost);

    _context.Outputs.ShouldBe([
      new InGameAudioLogicState.Output.StopGameMusic(),
      new InGameAudioLogicState.Output.PlayPlayerDied()
    ]);
  }

  [Fact]
  public void OnGameEndedOther()
  {
    _state.OnGameEnded(GameOverReason.Won);

    _context.Outputs.ShouldBe([
      new InGameAudioLogicState.Output.StopGameMusic()
    ]);
  }

  [Fact]
  public void OnJumped()
  {
    _state.OnJumped();

    _context.Outputs.ShouldBe([
      new InGameAudioLogicState.Output.PlayJump()
    ]);
  }

  [Fact]
  public void OnMainMenuEntered()
  {
    _state.OnMainMenuEntered();

    _context.Outputs.ShouldBe([
      new InGameAudioLogicState.Output.PlayMainMenuMusic()
    ]);
  }

  [Fact]
  public void OnGameEntered()
  {
    _state.OnGameEntered();

    _context.Outputs.ShouldBe([
      new InGameAudioLogicState.Output.PlayGameMusic()
    ]);
  }
}
