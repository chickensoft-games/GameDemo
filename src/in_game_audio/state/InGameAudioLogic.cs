namespace GameDemo;

using System;
using System.Collections.Generic;
using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;
using Chickensoft.LogicBlocks.Auto;

public interface IInGameAudioLogic : ILogicBlock;

[Meta]
public partial class InGameAudioLogic :
  AutoBlock, IInGameAudioLogic
{
  public override Type GetInitialState() => typeof(BaseState);

  public InGameAudioLogic()
  {
    Preallocate<BaseState>();
  }

  public override IEnumerable<IDisposable> OnStartSubscriptions()
  {
    yield return Get<IAppRepo>().AutoChannel.Bind()
      .On((in IAppRepo.MainMenuEntered _) => (State as BaseState)?.OnMainMenuEntered())
      .On((in IAppRepo.GameEntered _) => (State as BaseState)?.OnGameEntered());
    yield return Get<IGameRepo>().AutoChannel.Bind()
      .On((in IGameRepo.CoinCollectionStarted _) => (State as BaseState)?.OnCoinCollected())
      .On((in IGameRepo.JumpshroomUsed _) => (State as BaseState)?.OnJumpshroomUsed())
      .On((in IGameRepo.Ended message) => (State as BaseState)?.OnGameEnded(message.Reason))
      .On((in IGameRepo.Jumped _) => (State as BaseState)?.OnJumped());
  }

  public static class Output
  {
    public readonly record struct PlayCoinCollected;

    public readonly record struct PlayBounce;

    public readonly record struct PlayPlayerDied;

    public readonly record struct PlayJump;

    public readonly record struct PlayMainMenuMusic;

    public readonly record struct PlayGameMusic;

    public readonly record struct StopGameMusic;
  }
}
