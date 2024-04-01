namespace GameDemo;

using Chickensoft.GodotNodeInterfaces;
using Chickensoft.PowerUps;
using Godot;
using SuperNodes.Types;

public interface ILegend : IControl;

[SuperNode(typeof(AutoNode))]
public partial class Legend : Control, ILegend {
  // Called when the node enters the scene tree for the first time.

  public override partial void _Notification(int what);
  private string _currentController = Controller.Current;
  [Node] public ITextureRect ConfirmIcon { get; set; } = default!;

  public void OnReady() =>
    Input.JoyConnectionChanged += UpdateConfirmIconTexture;

  public void OnExitTree() =>
    Input.JoyConnectionChanged -= UpdateConfirmIconTexture;

  private void UpdateConfirmIconTexture(long device, bool connected) {
    _currentController = Controller.Current;
    ConfirmIcon.Texture =
      (Texture2D)ResourceLoader.Load(Controller.ConfirmPath);
  }


  public override void _Input(InputEvent @event) {
    base._Input(@event);

    if (InputUtilities.ValidInput(@event) is null) {
      return;
    }

    Controller.HandleCurrentControllers(@event);

    if (@event is InputEventJoypadButton or
        InputEventJoypadMotion or InputEventKey &&
       _currentController != Controller.Current) {
      _currentController = Controller.Current;
      ConfirmIcon.Texture =
        (Texture2D)ResourceLoader.Load(Controller.ConfirmPath);
    }
  }
}
