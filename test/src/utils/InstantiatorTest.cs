namespace GameDemo.Tests;

using Chickensoft.GoDotTest;
using Godot;
using Shouldly;

public class InstantiatorTest : TestClass
{
  public InstantiatorTest(Node testScene) : base(testScene) { }

  [Test]
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
