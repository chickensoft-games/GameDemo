namespace GameDemo.Tests;

using System.Threading.Tasks;
using Chickensoft.GoDotTest;
using Godot;
using GodotTestDriver;
using Shouldly;

public class DimmableAudioStreamPlayerTest : TestClass {
  private DimmableAudioStreamPlayer _player = default!;
  private Fixture _fixture = default!;

  public DimmableAudioStreamPlayerTest(Node testScene) : base(testScene) { }

  [Setup]
  public async Task Setup() {
    // This node has to be tested in the scene tree since you can't create
    // tweens outside of the scene tree.
    _player = new();
    _fixture = new(TestScene.GetTree());

    await _fixture.AddToRoot(_player);
  }

  [Cleanup]
  public async Task Cleanup() {
    await _fixture.Cleanup();
  }

  [Test]
  public void Initializes() {
    _player.VolumeDb = -1f;
    _player._Ready();

    _player.InitialVolumeDb.ShouldBe(-1f);
    _player.VolumeDb.ShouldBe(DimmableAudioStreamPlayer.VOLUME_DB_INAUDIBLE);
  }

  [Test]
  public void FadesIn() {
    _player.FadeIn();
    _player.FadeTween.ShouldNotBeNull();
    _player.FadeIn();
    _player.FadeTween.Kill();
  }

  [Test]
  public void FadesOut() {
    _player.FadeOut();
    _player.FadeTween.ShouldNotBeNull();
    _player.FadeOut();
    _player.FadeTween.Kill();
  }
}
