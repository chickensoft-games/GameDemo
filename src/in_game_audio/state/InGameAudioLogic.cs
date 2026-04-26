namespace GameDemo;

using System;
using System.Collections.Generic;
using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;

public interface IInGameAudioLogic : ILogicBlock;

[Meta]
public partial class InGameAudioLogic :
  LogicBlock, IInGameAudioLogic
{
  public override Type GetInitialState() => typeof(BaseState);

  public InGameAudioLogic()
  {
    Set(new BaseState());
  }

  public override IEnumerable<IDisposable> OnStartSubscriptions()
  {
    yield return Get<IAppRepo>().AutoChannel.Bind()
      .On((in IAppRepo.MainMenuEntered _) => State?.Output(new Output.PlayMainMenuMusic()))
      .On((in IAppRepo.GameEntered _) => State?.Output(new Output.PlayGameMusic()));
    yield return Get<IGameRepo>().AutoChannel.Bind()
      .On((in IGameRepo.CoinCollectionStarted _) => State?.Output(new Output.PlayCoinCollected()))
      .On((in IGameRepo.JumpshroomUsed _) => State?.Output(new Output.PlayBounce()))
      .On((in IGameRepo.Ended message) =>
      {
        State?.Output(new Output.StopGameMusic());

        if (message.Reason is not GameOverReason.Lost)
        {
          return;
        }

        State?.Output(new Output.PlayPlayerDied());
      })
      .On((in IGameRepo.Jumped _) => State?.Output(new Output.PlayJump()));
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

  [Meta]
  public partial record BaseState : LogicBlockState;
}
