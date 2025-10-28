namespace GameDemo.Tests;

using System.Diagnostics.CodeAnalysis;
using Chickensoft.AutoInject;
using Chickensoft.GodotNodeInterfaces;
using Chickensoft.GoDotTest;
using Godot;
using Moq;
using Shouldly;

[
  SuppressMessage(
    "Design",
    "CA1001",
    Justification = "Disposable field is Godot object; Godot will dispose"
  )
]
public class InGameAudioTest : TestClass
{
  private Mock<IAppRepo> _appRepo = default!;
  private Mock<IGameRepo> _gameRepo = default!;
  private Mock<IInGameAudioLogic> _logic = default!;

  private InGameAudioLogic.IFakeBinding _binding = default!;

  private Mock<IAudioStreamPlayer> _coinCollected = default!;
  private Mock<IAudioStreamPlayer> _bounce = default!;
  private Mock<IAudioStreamPlayer> _playerDied = default!;
  private Mock<IAudioStreamPlayer> _playerJumped = default!;
  private Mock<IDimmableAudioStreamPlayer> _mainMenuMusic = default!;
  private Mock<IDimmableAudioStreamPlayer> _gameMusic = default!;
  private InGameAudio _audio = default!;

  public InGameAudioTest(Node testScene) : base(testScene) { }

  [Setup]
  public void Setup()
  {
    _appRepo = new Mock<IAppRepo>();
    _gameRepo = new Mock<IGameRepo>();
    _logic = new Mock<IInGameAudioLogic>();
    _binding = InGameAudioLogic.CreateFakeBinding();
    _coinCollected = new Mock<IAudioStreamPlayer>();
    _bounce = new Mock<IAudioStreamPlayer>();
    _playerDied = new Mock<IAudioStreamPlayer>();
    _playerJumped = new Mock<IAudioStreamPlayer>();
    _mainMenuMusic = new Mock<IDimmableAudioStreamPlayer>();
    _gameMusic = new Mock<IDimmableAudioStreamPlayer>();

    _logic.Setup(logic => logic.Bind()).Returns(_binding);

    _audio = new InGameAudio
    {
      InGameAudioLogic = _logic.Object,
      CoinCollected = _coinCollected.Object,
      Bounce = _bounce.Object,
      PlayerDied = _playerDied.Object,
      PlayerJumped = _playerJumped.Object,
      MainMenuMusic = _mainMenuMusic.Object,
      GameMusic = _gameMusic.Object
    };

    (_audio as IAutoInit).IsTesting = true;

    _audio.FakeDependency(_appRepo.Object);
    _audio.FakeDependency(_gameRepo.Object);

    _audio._Notification(-1);
  }

  [Test]
  public void Initializes()
  {
    _audio.Setup();

    _audio.InGameAudioLogic.ShouldBeOfType<InGameAudioLogic>();
  }

  [Test]
  public void OnExitTree()
  {
    _logic.Reset();
    _logic.Setup(logic => logic.Stop());
    _audio.InGameAudioBinding = _binding;

    _audio.OnExitTree();

    _logic.VerifyAll();
  }

  [Test]
  public void PlaysMainMenuMusic()
  {
    _logic.Setup(logic => logic.Start());
    _gameMusic.Setup(music => music.FadeOut());
    _mainMenuMusic.Setup(music => music.Stop());
    _mainMenuMusic.Setup(music => music.FadeIn());

    _audio.OnResolved();

    _binding.Output(new InGameAudioLogic.Output.PlayMainMenuMusic());

    _logic.VerifyAll();
    _gameMusic.VerifyAll();
    _mainMenuMusic.VerifyAll();
  }

  [Test]
  public void PlaysGameMusic()
  {
    _logic.Setup(logic => logic.Start());
    _gameMusic.Setup(music => music.Stop());
    _gameMusic.Setup(music => music.FadeIn());
    _mainMenuMusic.Setup(music => music.FadeOut());

    _audio.OnResolved();

    _binding.Output(new InGameAudioLogic.Output.PlayGameMusic());

    _logic.VerifyAll();
    _gameMusic.VerifyAll();
    _mainMenuMusic.VerifyAll();
  }

  [Test]
  public void StopsGameMusic()
  {
    _logic.Setup(logic => logic.Start());
    _gameMusic.Setup(music => music.FadeOut());

    _audio.OnResolved();

    _binding.Output(new InGameAudioLogic.Output.StopGameMusic());

    _gameMusic.VerifyAll();
  }

  [Test]
  public void PlaysSounds()
  {
    _logic.Setup(logic => logic.Start());
    _coinCollected.Setup(sfx => sfx.Play(0));
    _bounce.Setup(sfx => sfx.Play(0));
    _playerDied.Setup(sfx => sfx.Play(0));
    _playerJumped.Setup(sfx => sfx.Play(0));

    _audio.OnResolved();

    _binding.Output(new InGameAudioLogic.Output.PlayCoinCollected());
    _binding.Output(new InGameAudioLogic.Output.PlayBounce());
    _binding.Output(new InGameAudioLogic.Output.PlayPlayerDied());
    _binding.Output(new InGameAudioLogic.Output.PlayJump());

    _coinCollected.VerifyAll();
    _bounce.VerifyAll();
    _playerDied.VerifyAll();
    _playerJumped.VerifyAll();
  }
}
