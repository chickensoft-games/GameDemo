namespace GameDemo;

using Chickensoft.AutoInject;
using Chickensoft.GodotNodeInterfaces;
using Chickensoft.Introspection;
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

  public InGameAudioLogic.IBinding InGameAudioBinding { get; set; } = default!;

  #endregion State

  public void Setup() => InGameAudioLogic = new InGameAudioLogic();

  public void OnResolved()
  {
    InGameAudioLogic.Set(AppRepo);
    InGameAudioLogic.Set(GameRepo);

    InGameAudioBinding = InGameAudioLogic.Bind();

    InGameAudioBinding
      .Handle((in InGameAudioLogic.Output.PlayCoinCollected _) =>
        CoinCollected.Play()
      )
      .Handle((in InGameAudioLogic.Output.PlayBounce _) => Bounce.Play()
      )
      .Handle((in InGameAudioLogic.Output.PlayPlayerDied _) => PlayerDied.Play()
      )
      .Handle((in InGameAudioLogic.Output.PlayJump _) =>
        PlayerJumped.Play()
      )
      .Handle((in InGameAudioLogic.Output.PlayMainMenuMusic _) =>
        StartMainMenuMusic()
      )
      .Handle((in InGameAudioLogic.Output.PlayGameMusic _) => StartGameMusic())
      .Handle((in InGameAudioLogic.Output.StopGameMusic _) =>
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
