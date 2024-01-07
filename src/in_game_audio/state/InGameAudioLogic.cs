namespace GameDemo;

using Chickensoft.LogicBlocks;
using Chickensoft.LogicBlocks.Generator;

public interface IInGameAudioLogic : ILogicBlock<InGameAudioLogic.IState>;

[StateMachine]
public partial class InGameAudioLogic :
  LogicBlock<InGameAudioLogic.IState>, IInGameAudioLogic {
  public override IState GetInitialState() => new State();

  public InGameAudioLogic(IAppRepo appRepo, IGameRepo gameRepo) {
    Set(appRepo);
    Set(gameRepo);
  }

  public static class Output {
    public readonly record struct PlayCoinCollected;

    public readonly record struct PlayBounce;

    public readonly record struct PlayPlayerDied;

    public readonly record struct PlayJump;

    public readonly record struct PlayMainMenuMusic;

    public readonly record struct PlayGameMusic;

    public readonly record struct StopGameMusic;
  }
}
