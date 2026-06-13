namespace GameDemo.Tests;

public static class Constants
{
  public const string HEADLESS = "GodotHeadless";
  public const string GODOT = "Godot";
}

[CollectionDefinition(Constants.HEADLESS, DisableParallelization = true)]
public class GodotHeadlessCollection : ICollectionFixture<GodotHeadlessFixture>;

[CollectionDefinition(Constants.GODOT, DisableParallelization = true)]
public class GodotCollection : ICollectionFixture<GodotFixture>;
