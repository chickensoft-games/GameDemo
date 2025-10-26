namespace GameDemo.Tests;

using System.Diagnostics.CodeAnalysis;
using Chickensoft.GoDotTest;
using Godot;
using Shouldly;

[
  SuppressMessage(
    "Design",
    "CA1001",
    Justification = "Disposable field is disposed in last test"
  )
]
public class AppRepoTest : TestClass
{
  private AppRepo _repo = default!;

  public AppRepoTest(Node testScene) : base(testScene) { }

  [Setup]
  public void Setup() => _repo = new();

  [Cleanup]
  public void Cleanup() => _repo.Dispose();

  [Test]
  public void Initializes()
  {
    var repo = new AppRepo();
    repo.ShouldBeAssignableTo<IAppRepo>();
  }

  [Test]
  public void SkipSplashScreen()
  {
    var called = false;

    void splashScreenSkipped() => called = true;

    // invoke event without handlers to cover null check
    _repo.SkipSplashScreen();

    _repo.SplashScreenSkipped += splashScreenSkipped;

    _repo.SkipSplashScreen();

    called.ShouldBe(true);
  }

  [Test]
  public void OnMainMenuEnteredInvokesEvent()
  {
    var called = 0;

    void onMainMenuEntered() => called++;

    _repo.OnMainMenuEntered();
    _repo.MainMenuEntered += onMainMenuEntered;
    _repo.OnMainMenuEntered();

    called.ShouldBe(1);
  }

  [Test]
  public void OnEnterGameInvokesEvent()
  {
    var called = 0;

    void onEnterGame() => called++;

    _repo.OnEnterGame();
    _repo.GameEntered += onEnterGame;
    _repo.OnEnterGame();

    called.ShouldBe(1);
  }

  [Test]
  public void OnExitGameInvokesEventWithPostGameAction()
  {
    var called = 0;
    var action = PostGameAction.GoToMainMenu;

    void onExitGame(PostGameAction a)
    {
      called++;
      a.ShouldBe(action);
    }

    _repo.OnExitGame(action);
    _repo.GameExited += onExitGame;
    _repo.OnExitGame(action);

    called.ShouldBe(1);
  }

  [Test]
  public void Disposes()
  {
    Should.NotThrow(_repo.Dispose);
    // Redundant dispose shouldn't do anything.
    Should.NotThrow(_repo.Dispose);
  }
}
