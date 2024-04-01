namespace GameDemo.Tests;

using Chickensoft.GodotNodeInterfaces;
using Chickensoft.GoDotTest;
using Godot;
using Moq;
using Shouldly;

public class LegendTest : TestClass {

  private Legend _legend = default!;

  private Mock<ITextureRect> _confirmIcon = default!;

  public LegendTest(Node testScene) : base(testScene) { }

  [Setup]
  public void Setup() {
    _confirmIcon = new Mock<ITextureRect>() {
      Object = {
        Texture = new Texture2D() {
          ResourceName = "ConfirmIcon",
          ResourceLocalToScene = true,
          ResourcePath = "MarginContainer/HBoxContainer/ConfirmIcon"
        }
      }
    };
    _legend = new Legend {
      ConfirmIcon = _confirmIcon.Object
    };
  }

  [Test]
  public void Onready() {
    _legend.OnReady();

    _legend.OnExitTree();
  }

  [Test]
  public void InputPressed() {
    var keyboardEvent = new InputEventKey {
      KeyLabel = Key.W,
      Pressed = true
    };
    Input.ParseInputEvent(keyboardEvent);

    _legend._Input(keyboardEvent);
    _legend._Input(default!);

    //_confirmIcon.Object.Texture.ResourcePath.ShouldBe("res://src/shared_ui/assets/Space_Key_Light.png");
  }
}
