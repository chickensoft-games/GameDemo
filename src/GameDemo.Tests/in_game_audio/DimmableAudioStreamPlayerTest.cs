namespace GameDemo.Tests;

using System.Diagnostics.CodeAnalysis;
using Shouldly;

[
  SuppressMessage(
    "Design",
    "CA1001",
    Justification = "Disposable field is added to TestDriver fixture"
  )
]
[Collection("GodotHeadless")]
public class DimmableAudioStreamPlayerTest : IDisposable
{
  private readonly DimmableAudioStreamPlayer _player = new();

  public DimmableAudioStreamPlayerTest(GodotHeadlessFixture godot)
  {
    godot.Tree.Root.AddChild(_player);
  }

  public void Dispose()
  {
    _player.QueueFree();
    _player.Dispose();
  }

  [Fact]
  public void Initializes()
  {
    _player.VolumeDb = -1f;
    _player._Ready();

    _player.InitialVolumeDb.ShouldBe(-1f);
    _player.VolumeDb.ShouldBe(DimmableAudioStreamPlayer.VOLUME_DB_INAUDIBLE);
  }

  [Fact]
  public void FadesIn()
  {
    _player.FadeIn();
    _player.FadeTween.ShouldNotBeNull();
    _player.FadeIn();
    _player.FadeTween.Kill();
  }

  [Fact]
  public void FadesOut()
  {
    _player.FadeOut();
    _player.FadeTween.ShouldNotBeNull();
    _player.FadeOut();
    _player.FadeTween.Kill();
  }
}
