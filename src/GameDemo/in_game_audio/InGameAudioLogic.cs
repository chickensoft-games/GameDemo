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
  public InGameAudioLogic()
  {
    Preallocate<InGameAudioLogicState>();
  }

  public override IEnumerable<IDisposable> OnStartSubscriptions()
  {
    yield return Get<IAppRepo>().AutoChannel.Bind()
      .On((in IAppRepo.MainMenuEntering _) => (State as InGameAudioLogicState)?.OnMainMenuEntered())
      .On((in IAppRepo.GameEntering _) => (State as InGameAudioLogicState)?.OnGameEntered());
    yield return Get<IGameRepo>().AutoChannel.Bind()
      .On((in IGameRepo.CoinCollectionStarted _) => (State as InGameAudioLogicState)?.OnCoinCollected())
      .On((in IGameRepo.JumpshroomUsed _) => (State as InGameAudioLogicState)?.OnJumpshroomUsed())
      .On((in IGameRepo.Ended message) => (State as InGameAudioLogicState)?.OnGameEnded(message.Reason))
      .On((in IGameRepo.Jumped _) => (State as InGameAudioLogicState)?.OnJumped());
  }
}
