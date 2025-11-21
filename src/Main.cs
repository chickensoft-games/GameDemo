namespace GameDemo;

using Godot;
using Chickensoft.GameTools.Displays;

#if RUN_TESTS
using System.Reflection;
using Chickensoft.GoDotTest;
#endif

// This entry-point file is responsible for determining if we should run tests.
//
// If you want to edit your game's main entry-point, please see Game.tscn and
// Game.cs instead.

public partial class Main : Node2D
{
  public Vector2I DesignResolution => Display.UHD4k;
#if RUN_TESTS
  public TestEnvironment Environment = default!;
#endif

  public override void _Ready()
  {
    // Correct any erroneous scaling and guess sensible defaults.
    GetWindow().LookGood(WindowScaleBehavior.UIProportional, DesignResolution);

#if RUN_TESTS
    // If this is a debug build, use GoDotTest to examine the
    // command line arguments and determine if we should run tests.
    Environment = TestEnvironment.From(OS.GetCmdlineArgs());
    if (Environment.ShouldRunTests)
    {
      CallDeferred(nameof(RunTests));
      return;
    }
#endif

    // If we don't need to run tests, we can just switch to the game scene.
    CallDeferred(nameof(StartApp));
  }

  private void StartApp() =>
    GetTree().ChangeSceneToFile("res://src/app/App.tscn");

#if RUN_TESTS
  private void RunTests()
    => _ = GoTest.RunTests(Assembly.GetExecutingAssembly(), this, Environment);
#endif
}
