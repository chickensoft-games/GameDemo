namespace GameDemo;

using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Chickensoft.AutoInject;
using Chickensoft.Collections;
using Chickensoft.GodotNodeInterfaces;
using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;
using Chickensoft.LogicBlocks.Auto;
using Chickensoft.SaveFileBuilder;
using Chickensoft.Serialization;
using Chickensoft.Serialization.Godot;
using Chickensoft.UMLGenerator;
using Godot;

public interface IApp : ICanvasLayer, IProvide<IAppRepo>, IProvide<ISaveFile>;

[Meta(typeof(IAutoNode))]
[ClassDiagram(UseVSCodePaths = true)]
public partial class App : CanvasLayer, IApp
{
  public override void _Notification(int what) => this.Notify(what);

  #region Constants

  public const string GAME_SCENE_PATH = "res://src/game/Game.tscn";

  #endregion Constants

  #region External

  public IGame Game { get; set; } = default!;
  public IInstantiator Instantiator { get; set; } = default!;

  #endregion External

  #region Save

  public ISaveFile SaveFile { get; set; } = Chickensoft.SaveFileBuilder.SaveFile.CreateGZipJsonFile(
    Path.Join(OS.GetUserDataDir(), "game.json.gz"),

    // Create a standard JsonSerializerOptions with our introspective type
    // resolver and the logic blocks converter.
    new JsonSerializerOptions()
    {
      Converters = {
        new SerializableTypeConverter(new Blackboard())
      },
      TypeInfoResolver = new SerializableTypeResolver(),
      WriteIndented = true
    }
  );

  #endregion Save

  #region Provisions

  IAppRepo IProvide<IAppRepo>.Value() => AppRepo;
  ISaveFile IProvide<ISaveFile>.Value() => SaveFile;

  #endregion Provisions

  #region State

  public IAppRepo AppRepo { get; set; } = default!;
  public IAppLogic AppLogic { get; set; } = default!;

  public LogicBlock.Binding AppBinding { get; set; } = default!;

  #endregion State

  #region Nodes

  [Node] public IMenu Menu { get; set; } = default!;
  [Node] public ISubViewport GamePreview { get; set; } = default!;
  [Node] public IColorRect BlankScreen { get; set; } = default!;
  [Node] public IAnimationPlayer AnimationPlayer { get; set; } = default!;
  [Node] public ISplash Splash { get; set; } = default!;

  #endregion Nodes

  public void Initialize()
  {
    Instantiator = new Instantiator(GetTree());
    AppRepo = new AppRepo();
    AppLogic = new AppLogic();
    AppLogic.Set(AppRepo);
    AppLogic.Set(new AppLogic.Data());

    // Listen for various menu signals. Each of these just translate to an input
    // for the overall app's state machine.
    Menu.NewGame += OnNewGame;
    Menu.LoadGame += OnLoadGame;
    Menu.DeleteGame += OnDeleteGame;

    AnimationPlayer.AnimationFinished += OnAnimationFinished;

    this.Provide();
  }

  public void OnReady()
  {
    // Tell our type type resolver about the Godot-specific converters.
    GodotSerialization.Setup();
    LogicBlockSerialization.Setup();

    AppBinding = AppLogic.Bind()
      .OnOutput((in AppLogicState.Output.ShowSplashScreen _) =>
      {
        HideMenus();
        BlankScreen.Hide();
        Splash.Show();
      })
      .OnOutput((in AppLogicState.Output.HideSplashScreen _) =>
      {
        BlankScreen.Show();
        FadeToBlack();
      })
      .OnOutput((in AppLogicState.Output.RemoveExistingGame _) =>
      {
        GamePreview.RemoveChildEx(Game);
        Game.QueueFree();
        Game = default!;
      })
      .OnOutput((in AppLogicState.Output.SetupGameScene _) =>
      {
        Game = Instantiator.LoadAndInstantiate<Game>(GAME_SCENE_PATH);
        GamePreview.AddChildEx(Game);

        Instantiator.SceneTree.Paused = false;
      })
      .OnOutput((in AppLogicState.Output.ShowMainMenu _) => ShowMainMenu().AsTask())
      .OnOutput((in AppLogicState.Output.FadeToBlack _) => FadeToBlack())
      .OnOutput((in AppLogicState.Output.ShowGame _) =>
      {
        HideMenus();
        FadeInFromBlack();
      })
      .OnOutput((in AppLogicState.Output.HideGame _) => FadeToBlack())
      .OnOutput((in AppLogicState.Output.StartLoadingSaveFile _) => LoadSaveFile().AsTask())
      .OnOutput((in AppLogicState.Output.StartDeletingSaveFile _) => DeleteSaveFile().AsTask());

    // Enter the first state to kick off the binding side effects.
    AppLogic.Start<AppLogicState.SplashScreen>();
  }

  public void OnNewGame() => AppLogic.Input(new AppLogicState.Input.NewGame());

  public void OnLoadGame() => AppLogic.Input(new AppLogicState.Input.LoadGame());

  public void OnDeleteGame() => AppLogic.Input(new AppLogicState.Input.DeleteGame());

  public void OnAnimationFinished(StringName animation)
  {
    // There's only two animations :)
    // We don't care what state we're in — we just tell the current state what's
    // happened and it will do the right thing.

    if (animation == "fade_in")
    {
      AppLogic.Input(new AppLogicState.Input.FadeInFinished());
      BlankScreen.Hide();
      return;
    }

    AppLogic.Input(new AppLogicState.Input.FadeOutFinished());
  }

  public void FadeInFromBlack()
  {
    BlankScreen.Show();
    AnimationPlayer.Play("fade_in");
  }

  public void FadeToBlack()
  {
    BlankScreen.Show();
    AnimationPlayer.Play("fade_out");
  }

  public void HideMenus()
  {
    Splash.Hide();
    Menu.Hide();
  }

  public void OnExitTree()
  {
    // Cleanup things we own.
    AppLogic.Stop();
    AppBinding.Dispose();
    AppRepo.Dispose();

    Menu.NewGame -= OnNewGame;
    Menu.LoadGame -= OnLoadGame;
    Menu.DeleteGame -= OnDeleteGame;

    AnimationPlayer.AnimationFinished -= OnAnimationFinished;
  }

  private async ValueTask ShowMainMenu()
  {
    // Load everything while we're showing a black screen, then fade in.
    HideMenus();
    Menu.Show();
    Game.Show();

    Menu.SetGameExists(await SaveFile.ExistsAsync());

    FadeInFromBlack();
  }

  private async ValueTask LoadSaveFile()
  {
    if (await SaveFile.ExistsAsync()
      && await SaveFile.LoadAsync<GameData>() is { } data)
    {
      Game.Load(data);
    }

    AppLogic.Input(new AppLogicState.Input.SaveFileLoaded());
  }

  private async ValueTask DeleteSaveFile()
  {
    Menu.SetGameExists(false);
    await SaveFile.DeleteAsync();
  }
}
