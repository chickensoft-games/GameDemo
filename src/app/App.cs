namespace GameDemo;

using Chickensoft.AutoInject;
using Chickensoft.GodotNodeInterfaces;
using Chickensoft.PowerUps;
using Godot;
using SuperNodes.Types;

public interface IApp : ICanvasLayer, IProvide<IAppRepo>;

[SuperNode(typeof(AutoSetup), typeof(AutoNode), typeof(Provider))]
public partial class App : CanvasLayer, IApp {
  public override partial void _Notification(int what);

  #region Constants

  public const string GAME_SCENE_PATH = "res://src/game/Game.tscn";

  #endregion Constants

  #region External

  public IGame Game { get; set; } = default!;
  public IInstantiator Instantiator { get; set; } = default!;

  #endregion External

  #region Provisions

  IAppRepo IProvide<IAppRepo>.Value() => AppRepo;

  #endregion Provisions

  #region State

  public IAppRepo AppRepo { get; set; } = default!;
  public IAppLogic AppLogic { get; set; } = default!;

  public AppLogic.IBinding AppBinding { get; set; } = default!;

  #endregion State

  #region Nodes

  [Node] public IMenu Menu { get; set; } = default!;
  [Node] public ISubViewport GamePreview { get; set; } = default!;
  [Node] public IColorRect BlankScreen { get; set; } = default!;
  [Node] public IAnimationPlayer AnimationPlayer { get; set; } = default!;
  [Node] public ISplash Splash { get; set; } = default!;

  #endregion Nodes

  public void Setup() {
    Instantiator = new Instantiator(GetTree());
    AppRepo = new AppRepo();
    AppLogic = new AppLogic(AppRepo);

    // Listen for various menu signals. Each of these just translate to an input
    // for the overall app's state machine.
    Menu.Start += OnStart;

    AnimationPlayer.AnimationFinished += OnAnimationFinished;

    Provide();
  }

  public void OnReady() {
    AppBinding = AppLogic.Bind();

    AppBinding
      .Handle((in AppLogic.Output.ShowSplashScreen _) => {
        HideMenus();
        BlankScreen.Hide();
        Splash.Show();
      })
      .Handle((in AppLogic.Output.HideSplashScreen _) => {
        BlankScreen.Show();
        FadeToBlack();
      })
      .Handle((in AppLogic.Output.RemoveExistingGame _) => {
        GamePreview.RemoveChildEx(Game);
        Game.QueueFree();
        Game = default!;
      })
      .Handle((in AppLogic.Output.LoadGame _) => {
        Game = Instantiator.LoadAndInstantiate<Game>(GAME_SCENE_PATH);
        GamePreview.AddChildEx(Game);

        Instantiator.SceneTree.Paused = false;
      })
      .Handle((in AppLogic.Output.ShowMainMenu _) => {
        // Load everything while we're showing a black screen, then fade in.
        HideMenus();
        Menu.Show();
        Game.Show();

        FadeInFromBlack();
      })
      .Handle((in AppLogic.Output.FadeToBlack _) => FadeToBlack())
      .Handle((in AppLogic.Output.ShowGame _) => {
        HideMenus();
        FadeInFromBlack();
      })
      .Handle((in AppLogic.Output.HideGame _) => FadeToBlack());

    // Enter the first state to kick off the binding side effects.
    AppLogic.Start();
  }

  public void OnStart() => AppLogic.Input(new AppLogic.Input.StartGame());

  public void OnAnimationFinished(StringName animation) {
    // There's only two animations :)
    // We don't care what state we're in — we just tell the current state what's
    // happened and it will do the right thing.

    if (animation == "fade_in") {
      AppLogic.Input(new AppLogic.Input.FadeInFinished());
      BlankScreen.Hide();
      return;
    }

    AppLogic.Input(new AppLogic.Input.FadeOutFinished());
  }

  public void FadeInFromBlack() {
    BlankScreen.Show();
    AnimationPlayer.Play("fade_in");
  }

  public void FadeToBlack() {
    BlankScreen.Show();
    AnimationPlayer.Play("fade_out");
  }

  public void HideMenus() {
    Splash.Hide();
    Menu.Hide();
  }

  public void OnExitTree() {
    // Cleanup things we own.
    AppLogic.Stop();
    AppBinding.Dispose();
    AppRepo.Dispose();

    Menu.Start -= OnStart;

    AnimationPlayer.AnimationFinished -= OnAnimationFinished;
  }
}
