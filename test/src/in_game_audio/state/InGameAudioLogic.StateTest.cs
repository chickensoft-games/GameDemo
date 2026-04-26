namespace GameDemo.Tests;

using Chickensoft.GoDotTest;
using Chickensoft.LogicBlocks;
using Godot;
using Moq;
using Shouldly;

public class InGameAudioLogicStateTest : TestClass
{
  private StateTester _context = default!;
  private Mock<IAppRepo> _appRepo = default!;
  private Mock<IGameRepo> _gameRepo = default!;
  private InGameAudioLogic.BaseState _baseState = default!;

  public InGameAudioLogicStateTest(Node testScene) : base(testScene) { }

  [Setup]
  public void Setup()
  {
    _appRepo = new();
    _gameRepo = new();

    _baseState = new();
    _context = _baseState.Test();
    _context.Set(_appRepo.Object);
    _context.Set(_gameRepo.Object);
  }

  [Test]
  public void OnCoinCollectionStarted()
  {
    _baseState.Output(new InGameAudioLogic.Output.PlayCoinCollected());

    _context.Outputs.ShouldBe([
      new InGameAudioLogic.Output.PlayCoinCollected()
    ]);
  }

  [Test]
  public void OnJumpshroomUsed()
  {
    _baseState.Output(new InGameAudioLogic.Output.PlayBounce());

    _context.Outputs.ShouldBe([
      new InGameAudioLogic.Output.PlayBounce()
    ]);
  }

  [Test]
  public void OnGameEndedLost()
  {
    _baseState.Output(new InGameAudioLogic.Output.StopGameMusic());
    _baseState.Output(new InGameAudioLogic.Output.PlayPlayerDied());

    _context.Outputs.ShouldBe([
      new InGameAudioLogic.Output.StopGameMusic(),
      new InGameAudioLogic.Output.PlayPlayerDied()
    ]);
  }

  [Test]
  public void OnGameEndedOther()
  {
    _baseState.Output(new InGameAudioLogic.Output.StopGameMusic());

    _context.Outputs.ShouldBe([
      new InGameAudioLogic.Output.StopGameMusic()
    ]);
  }

  [Test]
  public void OnJumped()
  {
    _baseState.Output(new InGameAudioLogic.Output.PlayJump());

    _context.Outputs.ShouldBe([
      new InGameAudioLogic.Output.PlayJump()
    ]);
  }

  [Test]
  public void OnMainMenuEntered()
  {
    _baseState.Output(new InGameAudioLogic.Output.PlayMainMenuMusic());

    _context.Outputs.ShouldBe([
      new InGameAudioLogic.Output.PlayMainMenuMusic()
    ]);
  }

  [Test]
  public void OnGameEntered()
  {
    _baseState.Output(new InGameAudioLogic.Output.PlayGameMusic());

    _context.Outputs.ShouldBe([
      new InGameAudioLogic.Output.PlayGameMusic()
    ]);
  }
}
