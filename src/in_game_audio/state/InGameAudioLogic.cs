namespace GameDemo;

using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;

public interface IInGameAudioLogic : ILogicBlock<InGameAudioLogic.State>;

[Meta]
[LogicBlock(typeof(State))]
public partial class InGameAudioLogic :
  LogicBlock<InGameAudioLogic.State>, IInGameAudioLogic
{
  public override Transition GetInitialState() => To<State>();

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
