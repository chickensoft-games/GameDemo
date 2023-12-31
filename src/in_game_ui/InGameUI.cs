namespace GameDemo;

using System;
using Chickensoft.AutoInject;
using Chickensoft.GodotNodeInterfaces;
using Chickensoft.LogicBlocks;
using Chickensoft.PowerUps;
using Godot;
using SuperNodes.Types;

public interface IInGameUI : IControl {
  void SetCoinsLabel(int coins, int totalCoins);
}

[SuperNode(typeof(AutoNode), typeof(Dependent))]
public partial class InGameUI : Control, IInGameUI {
  public override partial void _Notification(int what);

  #region Dependencies

  [Dependency] public IAppRepo AppRepo => DependOn<IAppRepo>();
  [Dependency] public IGameRepo GameRepo => DependOn<IGameRepo>();

  #endregion Dependencies

  #region Nodes

  [Node] public ILabel CoinsLabel { get; set; } = default!;

  #endregion Nodes

  #region State

  public IInGameUILogic InGameUILogic { get; set; } = default!;

  public Logic<InGameUILogic.IState, Func<object, InGameUILogic.IState>, InGameUILogic.IState,
    Action<InGameUILogic.IState?>>.IBinding InGameUIBinding { get; set; } = default!;

  #endregion State

  public void Setup() =>
    InGameUILogic = new InGameUILogic(this, AppRepo, GameRepo);

  public void OnResolved() {
    InGameUIBinding = InGameUILogic.Bind();

    // TODO: Move the access to the game repo to the state machine.

    InGameUIBinding
      .Handle<InGameUILogic.Output.NumCoinsCollectedChanged>(
        output => SetCoinsLabel(
          output.NumCoinsCollected, GameRepo.NumCoinsAtStart.Value
        )
      )
      .Handle<InGameUILogic.Output.NumCoinsAtStartChanged>(
        output => SetCoinsLabel(
          GameRepo.NumCoinsCollected.Value, output.NumCoinsAtStart
        )
      );

    InGameUILogic.Start();
  }

  public void SetCoinsLabel(int coins, int totalCoins) => CoinsLabel.Text = $"{coins}/{totalCoins}";

  public void OnExitTree() {
    InGameUILogic.Stop();
    InGameUIBinding.Dispose();
  }
}
