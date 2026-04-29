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
    yield return SetupSubscriptions(Get<IAppRepo>(), () => State);
    yield return SetupSubscriptions(Get<IGameRepo>(), () => State);
  }

  public static IDisposable SetupSubscriptions(IAppRepo appRepo, Func<LogicBlockState?> stateFunc)
  {
    return appRepo.AutoChannel.Bind()
      .On((in IAppRepo.MainMenuEntered _) => stateFunc()?.Output(new Output.PlayMainMenuMusic()))
      .On((in IAppRepo.GameEntered _) => stateFunc()?.Output(new Output.PlayGameMusic()));
  }

  public static IDisposable SetupSubscriptions(IGameRepo gameRepo, Func<LogicBlockState?> stateFunc)
  {
    return gameRepo.AutoChannel.Bind()
      .On((in IGameRepo.CoinCollectionStarted _) => stateFunc()?.Output(new Output.PlayCoinCollected()))
      .On((in IGameRepo.JumpshroomUsed _) => stateFunc()?.Output(new Output.PlayBounce()))
      .On((in IGameRepo.Ended message) =>
      {
        stateFunc()?.Output(new Output.StopGameMusic());

        if (message.Reason is not GameOverReason.Lost)
        {
          return;
        }

        stateFunc()?.Output(new Output.PlayPlayerDied());
      })
      .On((in IGameRepo.Jumped _) => stateFunc()?.Output(new Output.PlayJump()));
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
