namespace GameDemo;

using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;

[Meta, StateDiagram]
public partial record InGameAudioLogicState : LogicBlockState
{
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

  public void OnCoinCollected() =>
    Output(new Output.PlayCoinCollected());

  public void OnJumpshroomUsed() => Output(new Output.PlayBounce());

  public void OnGameEnded(GameOverReason reason)
  {
    Output(new Output.StopGameMusic());

    if (reason is not GameOverReason.Lost)
    {
      return;
    }

    Output(new Output.PlayPlayerDied());
  }

  public void OnJumped() => Output(new Output.PlayJump());

  // TODO: Use a different sound system for menu sounds.

  public void OnMainMenuEntered() =>
    Output(new Output.PlayMainMenuMusic());

  public void OnGameEntered() => Output(new Output.PlayGameMusic());
}
