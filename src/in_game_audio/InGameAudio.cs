namespace GameDemo;

using Chickensoft.AutoInject;
using Chickensoft.GodotNodeInterfaces;
using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;
using Godot;

[Meta(typeof(IAutoNode))]
public partial class InGameAudio : Node
{
  public override void _Notification(int what) => this.Notify(what);

  #region Nodes

  [Node] public IAudioStreamPlayer CoinCollected { get; set; } = default!;
  [Node] public IAudioStreamPlayer Bounce { get; set; } = default!;
  [Node] public IAudioStreamPlayer PlayerDied { get; set; } = default!;
  [Node] public IAudioStreamPlayer PlayerJumped { get; set; } = default!;
  [Node] public IDimmableAudioStreamPlayer MainMenuMusic { get; set; } = default!;
  [Node] public IDimmableAudioStreamPlayer GameMusic { get; set; } = default!;

  #endregion Nodes

  #region Dependencies

  [Dependency] public IAppRepo AppRepo => this.DependOn<IAppRepo>();

  [Dependency] public IGameRepo GameRepo => this.DependOn<IGameRepo>();

  #endregion Dependencies

  #region State

  public IInGameAudioLogic InGameAudioLogic { get; set; } = default!;

  public LogicBlock.Binding InGameAudioBinding { get; set; } = default!;

  #endregion State

  public void Setup() => InGameAudioLogic = new InGameAudioLogic();

  public void OnResolved()
  {
    InGameAudioLogic.Set(AppRepo);
    InGameAudioLogic.Set(GameRepo);

    InGameAudioBinding = InGameAudioLogic.Bind();

    InGameAudioBinding
      .OnOutput((in InGameAudioLogicState.Output.PlayCoinCollected _) =>
        CoinCollected.Play()
      )
      .OnOutput((in InGameAudioLogicState.Output.PlayBounce _) => Bounce.Play()
      )
      .OnOutput((in InGameAudioLogicState.Output.PlayPlayerDied _) => PlayerDied.Play()
      )
      .OnOutput((in InGameAudioLogicState.Output.PlayJump _) =>
        PlayerJumped.Play()
      )
      .OnOutput((in InGameAudioLogicState.Output.PlayMainMenuMusic _) =>
        StartMainMenuMusic()
      )
      .OnOutput((in InGameAudioLogicState.Output.PlayGameMusic _) => StartGameMusic())
      .OnOutput((in InGameAudioLogicState.Output.StopGameMusic _) =>
        GameMusic.FadeOut()
      );

    InGameAudioLogic.Start();
  }

  public void OnExitTree()
  {
    InGameAudioLogic.Stop();
    InGameAudioBinding.Dispose();
  }

  public void StartMainMenuMusic()
  {
    GameMusic.FadeOut();
    MainMenuMusic.Stop();
    MainMenuMusic.FadeIn();
  }

  public void StartGameMusic()
  {
    MainMenuMusic.FadeOut();
    GameMusic.Stop();
    GameMusic.FadeIn();
  }
}
