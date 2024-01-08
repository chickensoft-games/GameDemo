namespace GameDemo;

using Chickensoft.AutoInject;
using Chickensoft.GodotNodeInterfaces;
using Chickensoft.PowerUps;
using Godot;
using SuperNodes.Types;

public interface IMap : INode3D {
  /// <summary>Get the number of coins in the world.</summary>
  int GetCoinCount();
}

[SuperNode(typeof(AutoNode), typeof(Dependent))]
public partial class Map : Node3D, IMap {
  public override partial void _Notification(int what);

  #region Nodes

  [Node] public INode3D Coins { get; set; } = default!;

  #endregion Nodes

  #region Dependencies

  [Dependency] public IGameRepo GameRepo => DependOn<IGameRepo>();

  #endregion Dependencies

  public int GetCoinCount() => Coins.GetChildCount();
}
