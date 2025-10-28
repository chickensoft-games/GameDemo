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
    Justification = "Disposable field is a Godot object; Godot will dispose"
  )
]
public class AppTest : TestClass
{
  private App _app = default!;
  private Mock<IAppRepo> _appRepo = default!;
  private Mock<IAppLogic> _logic = default!;

  private AppLogic.IFakeBinding _binding = default!;

  private Mock<IInstantiator> _instantiator = default!;
  private Mock<IGame> _game = default!;
  private Mock<IMenu> _menu = default!;
  private Mock<ISubViewport> _gamePreview = default!;
  private Mock<IColorRect> _blankScreen = default!;
  private Mock<IAnimationPlayer> _animationPlayer = default!;
  private Mock<ISplash> _splash = default!;

  public AppTest(Node testScene) : base(testScene) { }

  [Setup]
  public void Setup()
  {
    _appRepo = new();
    _logic = new();
    _binding = AppLogic.CreateFakeBinding();

    _instantiator = new();

    _game = new();
    _menu = new();
    _gamePreview = new();
    _blankScreen = new();
    _animationPlayer = new();
    _splash = new();

    _app = new()
    {
      AppRepo = _appRepo.Object,
      AppLogic = _logic.Object,
      Game = _game.Object,
      Instantiator = _instantiator.Object,
      Menu = _menu.Object,
      GamePreview = _gamePreview.Object,
      BlankScreen = _blankScreen.Object,
      AnimationPlayer = _animationPlayer.Object,
      Splash = _splash.Object
    };

    (_app as IAutoInit).IsTesting = true;

    _logic.Setup(logic => logic.Bind()).Returns(_binding);
    _logic.Setup(logic => logic.Start());
  }

  [Test]
  public void Initializes()
  {
    // Naturally, the app controls al ot of systems (mostly menus), so there's
    // quite a bit of setup to verify.

    _app.AppBinding = _binding;

    _app.Initialize();

    _app.Instantiator.ShouldBeOfType<Instantiator>();
    _app.AppRepo.ShouldBeOfType<AppRepo>();
    _app.AppLogic.ShouldBeOfType<AppLogic>();

    _menu.VerifyAdd(menu => menu.NewGame += _app.OnNewGame);

    _animationPlayer.VerifyAdd(
      player => player.AnimationFinished += _app.OnAnimationFinished
    );

    // Make sure the app provides dependency values to its descendants.
    (_app as IProvider).ProviderState.IsInitialized.ShouldBeTrue();
    (_app as IProvide<IAppRepo>).Value().ShouldBe(_app.AppRepo);

    // Make sure it cleans everything up.

    _app.OnExitTree();

    _menu.VerifyRemove(menu => menu.NewGame -= _app.OnNewGame);

    _animationPlayer.VerifyRemove(
      player => player.AnimationFinished -= _app.OnAnimationFinished
    );

    _app._Notification(-1);
  }

  [Test]
  public void ShowsSplashScreen()
  {
    SetupHideMenus();
    _blankScreen.Setup(blank => blank.Hide());
    _splash.Setup(splash => splash.Show());

    _app.OnReady();

    _binding.Output(new AppLogic.Output.ShowSplashScreen());

    _blankScreen.VerifyAll();
    _splash.VerifyAll();
  }

  [Test]
  public void HidesSplashScreen()
  {
    _blankScreen.Setup(blank => blank.Show());
    SetupFadeOut();

    _app.OnReady();

    _binding.Output(new AppLogic.Output.HideSplashScreen());

    _blankScreen.VerifyAll();
  }

  [Test]
  public void RemovesExistingGame()
  {
    _game.Setup(game => game.QueueFree());

    _app.Game = _game.Object;
    _gamePreview.Setup(preview => preview.RemoveChildEx(_game.Object));

    _app.OnReady();

    _binding.Output(new AppLogic.Output.RemoveExistingGame());

    _gamePreview.VerifyAll();

    _app.Game.ShouldBe(null);

    _game.VerifyAll();
  }

  [Test]
  public void LoadsGame()
  {
    var game = new Game();

    _instantiator.Setup(i => i.LoadAndInstantiate<Game>(It.IsAny<string>()))
      .Returns(game);
    _instantiator.Setup(i => i.SceneTree).Returns(TestScene.GetTree());

    _gamePreview.Setup(
      preview => preview.AddChildEx(game, false, Node.InternalMode.Disabled)
    );

    _app.OnReady();

    _binding.Output(new AppLogic.Output.SetupGameScene());

    _instantiator.VerifyAll();
    _gamePreview.VerifyAll();
  }

  [Test]
  public void ShowsMainMenu()
  {
    SetupHideMenus();
    _menu.Setup(menu => menu.Show());
    _game.Setup(game => game.Show());
    SetupFadeIn();

    _app.OnReady();

    _binding.Output(new AppLogic.Output.ShowMainMenu());

    _menu.VerifyAll();
    _game.VerifyAll();
    _blankScreen.VerifyAll();
    _animationPlayer.VerifyAll();
  }

  [Test]
  public void FadesOut()
  {
    SetupFadeOut();

    _app.OnReady();

    _binding.Output(new AppLogic.Output.FadeToBlack());

    VerifyFade();
  }

  [Test]
  public void ShowsGame()
  {
    SetupHideMenus();

    SetupFadeIn();

    _app.OnReady();

    _binding.Output(new AppLogic.Output.ShowGame());

    VerifyFade();
  }

  [Test]
  public void HidesGame()
  {
    SetupFadeOut();

    _app.OnReady();

    _binding.Output(new AppLogic.Output.HideGame());

    VerifyFade();
  }

  [Test]
  public void StartsLoadingSaveFile()
  {
    _app.OnReady();

    _binding.Output(new AppLogic.Output.StartLoadingSaveFile());

    _game.VerifyAdd(game => game.SaveFileLoaded += _app.OnSaveFileLoaded);
    _game.Verify(game => game.LoadExistingGame());
  }

  [Test]
  public void OnNewGameWorks()
  {
    _logic.Reset();
    _logic.Setup(logic => logic.Input(It.IsAny<AppLogic.Input.NewGame>()));
    _app.OnNewGame();
    _logic.VerifyAll();
  }

  [Test]
  public void OnLoadGameWorks()
  {
    _logic.Reset();
    _logic.Setup(logic => logic.Input(It.IsAny<AppLogic.Input.LoadGame>()));
    _app.OnLoadGame();
    _logic.VerifyAll();
  }

  [Test]
  public void OnAnimationFinishedRespondsToFadeInFinished()
  {
    _logic.Reset();
    _logic
      .Setup(logic => logic.Input(It.IsAny<AppLogic.Input.FadeInFinished>()));
    _blankScreen.Setup(screen => screen.Hide());

    _app.OnAnimationFinished("fade_in");

    _logic.VerifyAll();
    _blankScreen.VerifyAll();
  }

  [Test]
  public void OnAnimationFinishedRespondsToFadeOutFinished()
  {
    _logic.Reset();
    _logic
      .Setup(logic => logic.Input(It.IsAny<AppLogic.Input.FadeOutFinished>()));

    _app.OnAnimationFinished("fade_out");

    _logic.VerifyAll();
  }

  [Test]
  public void OnSaveFileLoaded()
  {

    _app.OnSaveFileLoaded();

    _logic.Verify(logic => logic.Input(It.IsAny<AppLogic.Input.SaveFileLoaded>()));

    _game.VerifyRemove(game => game.SaveFileLoaded -= _app.OnSaveFileLoaded);
  }

  // Mocking helpers

  private void SetupHideMenus()
  {
    _menu.Setup(menu => menu.Hide());
    _splash.Setup(splash => splash.Hide());
  }

  private void SetupFadeIn()
  {
    _blankScreen.Setup(blank => blank.Show());
    _animationPlayer.Setup(player => player.Play("fade_in", -1, 1, false));
  }

  private void SetupFadeOut()
  {
    _blankScreen.Setup(blank => blank.Show());
    _animationPlayer.Setup(player => player.Play("fade_out", -1, 1, false));
  }

  private void VerifyFade()
  {
    _blankScreen.VerifyAll();
    _animationPlayer.VerifyAll();
  }
}
