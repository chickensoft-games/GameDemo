namespace GameDemo.Tests;

using Godot;
using Shouldly;

public class InstantiatorTest(GodotHeadlessFixture godot)
{
  public InstantiatorTest(Node testScene) : base(testScene) { }

  [Fact]
  public void Instantiates()
  {
    var instantiator = new Instantiator(TestScene.GetTree());

    var scene = instantiator.LoadAndInstantiate<Node3D>(
      "res://src/in_game_ui/coin_scene/CoinScene.tscn"
    );

    scene.ShouldBeOfType<Node3D>();
    scene.QueueFree();
  }
}
