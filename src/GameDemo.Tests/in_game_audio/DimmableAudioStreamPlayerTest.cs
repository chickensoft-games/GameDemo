namespace GameDemo.Tests;

using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Godot;
using Shouldly;

[
  SuppressMessage(
    "Design",
    "CA1001",
    Justification = "Disposable field is added to TestDriver fixture"
  )
]
public class DimmableAudioStreamPlayerTest(GodotHeadlessFixture godot)
{
  private DimmableAudioStreamPlayer _player = default!;
  private Fixture _fixture = default!;

  public DimmableAudioStreamPlayerTest(Node testScene) : base(testScene) { }

  [Setup]
  public async Task Setup()
  {
    // This node has to be tested in the scene tree since you can't create
    // tweens outside of the scene tree.
    _player = new();
    _fixture = new(TestScene.GetTree());

    await _fixture.AddToRoot(_player);
  }

  [Cleanup]
  public async Task Cleanup() => await _fixture.Cleanup();

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
