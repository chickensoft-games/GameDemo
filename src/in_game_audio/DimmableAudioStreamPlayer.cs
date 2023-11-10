namespace GameDemo;

using Chickensoft.GodotNodeInterfaces;
using Godot;

public interface IDimmableAudioStreamPlayer : IAudioStreamPlayer {
  /// <summary>Fade this dimmable audio stream track in.</summary>
  public void FadeIn();
  /// <summary>Fade this dimmable audio stream track out.</summary>
  public void FadeOut();
}

public partial class DimmableAudioStreamPlayer :
  AudioStreamPlayer, IDimmableAudioStreamPlayer {
  #region Constants
  // -60 to -80 is considered inaudible for decibels.
  public const float VOLUME_DB_INAUDIBLE = -80f;
  public const double FADE_DURATION = 3d; // seconds
  #endregion Constants

  public ITween? FadeTween { get; set; }

  public float InitialVolumeDb;

  public override void _Ready() {
    InitialVolumeDb = VolumeDb;
    VolumeDb = VOLUME_DB_INAUDIBLE;
  }

  public void FadeIn() {
    SetupFade(InitialVolumeDb, Tween.EaseType.Out);
    Play();
  }

  public void FadeOut() {
    SetupFade(VOLUME_DB_INAUDIBLE, Tween.EaseType.In);
    FadeTween!.TweenCallback(Callable.From(Stop));
  }

  public void SetupFade(float volumeDb, Tween.EaseType ease) {
    FadeTween?.Kill();

    FadeTween = GodotInterfaces.Adapt<ITween>(CreateTween());

    FadeTween.TweenProperty(
      this,
      "volume_db",
      volumeDb,
      FADE_DURATION
    ).SetTrans(Tween.TransitionType.Circ).SetEase(ease);
  }
}
