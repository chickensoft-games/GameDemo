namespace GameDemo.Tests;

using Godot;
using Shouldly;

[Collection(Constants.Headless)]
public class InstantiatorTest(GodotHeadlessFixture godot)
{
  [Fact]
  public void Instantiates()
  {
    var instantiator = new Instantiator(godot.Tree);

    var scene = instantiator.LoadAndInstantiate<Node3D>(
      "res://in_game_ui/coin_scene/CoinScene.tscn"
    );

    scene.ShouldBeOfType<Node3D>();
    scene.QueueFree();
  }
}
