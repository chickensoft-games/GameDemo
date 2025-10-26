namespace GameDemo.Tests;

using System.Diagnostics.CodeAnalysis;
using Chickensoft.AutoInject;
using Chickensoft.GodotNodeInterfaces;
using Chickensoft.GoDotTest;
using Godot;
using Moq;

[
  SuppressMessage(
    "Design",
    "CA1001",
    Justification = "Disposable field is Godot object; Godot will dispose"
  )
]
public class SplashTest : TestClass
{
  private Mock<IAppRepo> _appRepo = default!;
  private Mock<IAnimationPlayer> _animationPlayer = default!;
  private Splash _splash = default!;

  public SplashTest(Node testScene) : base(testScene) { }

  [Setup]
  public void Setup()
  {
    _appRepo = new Mock<IAppRepo>();
    _animationPlayer = new Mock<IAnimationPlayer>();
    _splash = new Splash()
    {
      AnimationPlayer = _animationPlayer.Object
    };

    _splash.FakeDependency(_appRepo.Object);

    _splash._Notification(-1);
  }

  [Test]
  public void PlaysSplashScreen()
  {
    _splash.OnReady();

    _animationPlayer.VerifyAdd(
      player => player.AnimationFinished += _splash.OnAnimationFinished
    );

    _appRepo.Setup(repo => repo.SkipSplashScreen());
    _splash.OnAnimationFinished("splash");
    _appRepo.VerifyAll();

    _splash.OnExitTree();
    _animationPlayer.VerifyRemove(
      player => player.AnimationFinished -= _splash.OnAnimationFinished
    );
  }

  [Test]
  public void SkipsSplashScreen()
  {
    _splash.OnReady();

    _animationPlayer.VerifyAdd(
      player => player.AnimationFinished += _splash.OnAnimationFinished
    );

    // Make sure clicking skips it.
    var input = new InputEventMouseButton()
    {
      Pressed = true
    };

    _appRepo.Setup(repo => repo.SkipSplashScreen());
    _splash._Input(input);
    _appRepo.VerifyAll();
    _appRepo.Reset();

    // Make sure other inputs don't skip it.
    var otherInput = new InputEventMouseButton()
    {
      Pressed = false
    };

    var otherInput2 = new InputEventKey()
    {
      Pressed = true
    };
    _splash._Input(otherInput);
    _splash._Input(otherInput2);
    _appRepo.VerifyAll();
  }
}
