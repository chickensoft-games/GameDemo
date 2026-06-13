namespace GameDemo.Tests;

public class Constants
{
  public const string Headless = "GodotHeadless";
  public const string Godot = "Godot";
}

[CollectionDefinition(Constants.Headless, DisableParallelization = true)]
public class GodotHeadlessCollection : ICollectionFixture<GodotHeadlessFixture>;

[CollectionDefinition(Constants.Godot, DisableParallelization = true)]
public class GodotCollection : ICollectionFixture<GodotFixture>;
