namespace GameDemo;

using Chickensoft.AutoInject;
using Chickensoft.GodotNodeInterfaces;
using Godot;
using Chickensoft.Introspection;

public interface IInGameUI : IControl {
  void SetCoinsLabel(int coins, int totalCoins);
}

[Meta(typeof(IAutoNode))]
public partial class InGameUI : Control, IInGameUI {
  public override void _Notification(int what) => this.Notify(what);

  #region Dependencies

  [Dependency] public IAppRepo AppRepo => this.DependOn<IAppRepo>();
  [Dependency] public IGameRepo GameRepo => this.DependOn<IGameRepo>();

  #endregion Dependencies

  #region Nodes

  [Node] public ILabel CoinsLabel { get; set; } = default!;

  #endregion Nodes

  #region State

  public IInGameUILogic InGameUILogic { get; set; } = default!;

  public InGameUILogic.IBinding InGameUIBinding { get; set; } = default!;

  #endregion State

  public void Setup() {
    InGameUILogic = new InGameUILogic();
  }

  public void OnResolved() {
    InGameUILogic.Set(this);
    InGameUILogic.Set(AppRepo);
    InGameUILogic.Set(GameRepo);

    InGameUIBinding = InGameUILogic.Bind();

    InGameUIBinding
      .Handle((in InGameUILogic.Output.NumCoinsChanged output) =>
        SetCoinsLabel(
          output.NumCoinsCollected, output.NumCoinsAtStart
        )
      );

    InGameUILogic.Start();
  }

  public void SetCoinsLabel(int coins, int totalCoins) =>
    CoinsLabel.Text = $"{coins}/{totalCoins}";

  public void OnExitTree() {
    InGameUILogic.Stop();
    InGameUIBinding.Dispose();
  }
}
