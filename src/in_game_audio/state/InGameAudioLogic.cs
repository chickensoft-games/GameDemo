namespace GameDemo;

using Chickensoft.LogicBlocks;
using Chickensoft.LogicBlocks.Generator;

public interface IInGameAudioLogic : ILogicBlock<InGameAudioLogic.IState> { }

[StateMachine]
public partial class InGameAudioLogic :
  LogicBlock<InGameAudioLogic.IState>, IInGameAudioLogic {
  public override IState GetInitialState(IContext context) =>
    new State(context);

  public InGameAudioLogic(IAppRepo appRepo) {
    Set(appRepo);
  }

  public static class Output {
    public readonly record struct PlayCoinCollected;
    public readonly record struct PlayBounce;
    public readonly record struct PlayPlayerDied;
    public readonly record struct PlayJump;
    public readonly record struct PlayMainMenuMusic;
    public readonly record struct StopMainMenuMusic;
    public readonly record struct PlayGameMusic;
    public readonly record struct StopGameMusic;
  }
}
